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

namespace CSBlog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IBlogService _blogService;
        private readonly IImageService _imageService;

        public BlogPostsController(ApplicationDbContext context, 
                                   UserManager<BlogUser> userManager, 
                                   IBlogService blogService,
                                   IImageService imageService)
        {
            _context = context;
            _userManager = userManager;
            _blogService = blogService;
            _imageService = imageService;
        }

        // GET: BlogPosts
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            string userId = _userManager.GetUserId(User)!;

            List<BlogPost> blogPosts = new List<BlogPost>();

            // include isPublished == true, isDeleted == false
            blogPosts = await _context.BlogPosts
                                      .Include(c => c.Category)
                                      .Include(c => c.Tags)
                                      .ToListAsync();

            return View(blogPosts);
        }

        public async Task<IActionResult> AdminPage()
        {
            // include isPublished == true, isDeleted == false
            var blogPosts = await _context.BlogPosts
                                      .Include(c => c.Category)
                                      .Include(c => c.Tags)
                                      .ToListAsync();

            return View(blogPosts);
        }


        // GET: BlogPosts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public async Task<IActionResult> Create()
        {
            IEnumerable<Category> categoryList = await _context.Categories                                         
                                                               .ToListAsync();

            IEnumerable<Tag> tagsList = await _context.Tags
                                                      .ToListAsync();


            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["TagList"] = new MultiSelectList(_context.Tags, "Id", "Name");
            return View(new BlogPost());
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsDeleted,IsPublished,ImageFile,ImageData,ImageType,CategoryId")] BlogPost blogPost, IEnumerable<int> selected)
        {
            if (ModelState.IsValid)
            {
                // TODO: Image Service


                blogPost.Created = DataUtility.GetPostGresDate(DateTime.UtcNow);

                if (blogPost.ImageFile != null)
                {
                    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
                    blogPost.ImageType = blogPost.ImageFile.ContentType;
                }

                _context.Add(blogPost);
                await _context.SaveChangesAsync();

                // TODO: Add Service Call 
                // DONE

                await _blogService.AddBlogPostToTagsAsync(selected, blogPost.Id);


                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name");
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                                         .Include(c => c.Category)
                                         .Include(c => c.Tags)
                                         .FirstOrDefaultAsync(c => c.Id == id);


            IEnumerable<Category> categoryList = await _context.Categories
                                                   .ToListAsync();

            IEnumerable<Tag> tagsList = await _context.Tags
                                                      .ToListAsync();
            IEnumerable<int> currentTag = blogPost!.Tags.Select(c => c.Id);

            ViewData["TagList"] = new MultiSelectList(_context.Tags, "Id", "Name", currentTag);

            if (blogPost == null)
            {
                return NotFound();
            }
            ViewData["CategoryList"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,Content,Created,Updated,Slug,IsDeleted,IsPublished,ImageData,ImageType,CategoryId")] BlogPost blogPost, IEnumerable<int> selected)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    blogPost.Created = DataUtility.GetPostGresDate(blogPost.Created);
                    blogPost.Updated = DataUtility.GetPostGresDate(DateTime.UtcNow);

                    if (blogPost.ImageFile != null)
                    {
                        blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
                        blogPost.ImageType = blogPost.ImageFile.ContentType;
                    }


                    _context.Update(blogPost);
                    await _context.SaveChangesAsync();


                    if(selected != null)
                    {
                    await _blogService.RemoveAllBlogPostTagsAsync(blogPost.Id);

                    await _blogService.AddBlogPostToTagsAsync(selected, blogPost.Id);
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            if (_context.BlogPosts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BlogPosts'  is null.");
            }
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost != null)
            {
                _context.BlogPosts.Remove(blogPost);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
          return (_context.BlogPosts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
