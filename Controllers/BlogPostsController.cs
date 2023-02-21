using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CSBlog.Data;
using CSBlog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using CSBlog.Services.Interfaces;
using CSBlog.Services;
using CSBlog.Helpers;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace CSBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogPostsController : Controller
    {
        private readonly IImageService _imageService;
        private readonly IBlogPostService _blogPostService;
        private readonly UserManager<BlogUser> _userManager;

        public BlogPostsController(IImageService imageService,
                                   IBlogPostService blogPostService,
                                   UserManager<BlogUser> userManager)
        {
            _imageService = imageService;
            _blogPostService = blogPostService;
            _userManager = userManager;
        }

        // GET: BlogPosts
        public async Task<IActionResult> AdminPage()
        {
            // include isPublished == true, isDeleted == false
            var blogPosts = await _blogPostService.GetBlogPostsAsync();
            return View(blogPosts);
        }


        // GET: BlogPosts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var blogPost = await _blogPostService.GetBlogPostAsync(slug);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }


        // GET: BlogPosts/Create
        public async Task<IActionResult> Create()
        {

            ViewData["CategoryList"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");
            ViewData["TagList"] = new MultiSelectList(await _blogPostService.GetTagsAsync(), "Id", "Name");
            return View(new BlogPost());
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsDeleted,IsPublished,ImageFile,ImageData,ImageType,CategoryId")] BlogPost blogPost, string? stringTags)
        {
            ModelState.Remove("Slug");

            if (ModelState.IsValid)
            {
                // Slug BlogPost

                if (!await _blogPostService.ValidateSlugAsync(blogPost.Title!, blogPost.Id))
                {
                    ModelState.AddModelError("Title", "A similar Title or Slug is already in use.");

                    ViewData["CategoryList"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");
                    return View(blogPost);
                }

                blogPost.Slug = StringHelper.BlogSlug(blogPost.Title!);


                blogPost.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);

                if (blogPost.ImageFile != null)
                {
                    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
                    blogPost.ImageType = blogPost.ImageFile.ContentType;
                }
                await _blogPostService.AddBlogPostAsync(blogPost);

                // Service Call 

                if (!string.IsNullOrWhiteSpace(stringTags))
                {
                    // Add Tags to BlogPost
                    await _blogPostService.AddBlogPostToTagsAsync(stringTags, blogPost.Id);
                }



                return RedirectToAction(nameof(AdminPage));
            }

            ViewData["CategoryList"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _blogPostService.GetBlogPostAsync(id.Value);

            if (blogPost == null)
            {
                return NotFound();
            }


            IEnumerable<string> tagNames = blogPost.Tags.Select(t => t.Name!);
            ViewData["TagList"] = string.Join(", ", tagNames);


            ViewData["CategoryList"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name", blogPost.CategoryId);
            
            
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsDeleted,IsPublished,ImageData,ImageType,CategoryId")] BlogPost blogPost, string? stringTags)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    // Update Slug
                    if (!await _blogPostService.ValidateSlugAsync(blogPost.Title!, blogPost.Id))
                    {
                        ModelState.AddModelError("Title", "A similar Title or Slug is already in use.");

                        ViewData["CategoryList"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name");
                        return View(blogPost);
                    }
                    blogPost.Slug = StringHelper.BlogSlug(blogPost.Title!);


                    // Dates
                    blogPost.Created = DataUtility.GetPostGresDate(blogPost.Created);
                    blogPost.Updated = DataUtility.GetPostGresDate(DateTime.UtcNow);

                    // Image Service
                    if (blogPost.ImageFile != null)
                    {
                        blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
                        blogPost.ImageType = blogPost.ImageFile.ContentType;
                    }

                    // Update Blog
                    await _blogPostService.UpdateBlogPostAsync(blogPost);



                    // Tags
                    await _blogPostService.RemoveAllBlogPostTagsAsync(blogPost.Id);

                    if (!string.IsNullOrWhiteSpace(stringTags))
                    {
                        await _blogPostService.AddBlogPostToTagsAsync(stringTags, blogPost.Id);
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AdminPage));
            }
            ViewData["CategoryList"] = new SelectList(await _blogPostService.GetCategoriesAsync(), "Id", "Name", blogPost.CategoryId);
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _blogPostService.GetBlogPostAsync(id.Value);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _blogPostService.GetBlogPostAsync(id);

            if (blogPost != null)
            {
                await _blogPostService.DeleteBlogPostAsync(blogPost);
            }

            return RedirectToAction(nameof(AdminPage));
        }

        private async Task<bool> BlogPostExists(int id)
        {
            return (await _blogPostService.GetBlogPostsAsync()).Any(b => b.Id == id);
        }
    }
}
