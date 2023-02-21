using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CSBlog.Data;
using CSBlog.Models;
using CSBlog.Services;
using CSBlog.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CSBlog.Controllers
{
    public class CommentsController : Controller
    {
        private readonly IBlogPostService _blogPostService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;

        public CommentsController(IBlogPostService blogPostService, ApplicationDbContext context, UserManager<BlogUser> userManager)
        {
            _blogPostService = blogPostService;
            _context = context;
            _userManager = userManager;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var comments = await _blogPostService.GetCommentsAsync();
            return View(comments);
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _blogPostService.GetCommentAsync(id.Value);

            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            //string? userId = _userManager.GetUserId(User);
            // Get BlogPost ID

            // Automatically assign author to Author ID           
            //ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id");
            //ViewData["BlogPostId"] = new SelectList(await _blogPostService.GetBlogPostsAsync(), "Id", "Content");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Body,Created,Updated,UpdateReason,BlogPostId,AuthorId")] Comment comment, int blogPostId)
        {
            BlogPost? blogPost = await _blogPostService.GetBlogPostAsync(blogPostId);
            ModelState.Remove("AuthorId");

            if (ModelState.IsValid)
            {
                // Automatically assign author and blogpost
                comment.AuthorId = _userManager.GetUserId(User);
                comment.BlogPostId = blogPost!.Id;

                comment.Created = DateTime.UtcNow;


                _context.Add(comment);
                blogPost.Comments.Add(comment);

                //blogPost!.Comments.OrderByDescending(c => c.Created);

                await _context.SaveChangesAsync();

                //await _blogPostService.AddCommentAsync(comment);

                return RedirectToAction(blogPost.Slug, "Content");
                //return View();
            }

            return View(comment);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _blogPostService.GetCommentAsync(id.Value);

            if (comment == null)
            {
                return NotFound();
            }
            // Automatically assign author to Author ID
            //ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", comment.AuthorId);
            ViewData["BlogPostId"] = new SelectList(await _blogPostService.GetBlogPostsAsync(), "Id", "Content", comment.BlogPostId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Body,Created,Updated,UpdateReason,BlogPostId,AuthorId")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    comment.Created = DateTime.SpecifyKind(comment.Created, DateTimeKind.Utc);

                    await _blogPostService.UpdateCommentAsync(comment);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CommentExists(comment.Id))
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
            // Automatically assign author to Author ID
            //ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", comment.AuthorId);
            ViewData["BlogPostId"] = new SelectList(await _blogPostService.GetBlogPostsAsync(), "Id", "Content", comment.BlogPostId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _blogPostService.GetCommentAsync(id.Value);

            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _blogPostService.GetCommentAsync(id);
            
            if (comment != null)
            {
                await _blogPostService.DeleteCommentAsync(comment);
            }
            
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CommentExists(int id)
        {
          return (await _blogPostService.GetCommentsAsync()).Any(e => e.Id == id);
        }
    }
}
