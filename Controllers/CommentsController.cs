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
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace CSBlog.Controllers
{
    [Authorize(Roles = "Admin, Moderator")]
    public class CommentsController : Controller
    {
        private readonly IBlogPostService _blogPostService;
        private readonly UserManager<BlogUser> _userManager;

        public CommentsController(IBlogPostService blogPostService, UserManager<BlogUser> userManager)
        {
            _blogPostService = blogPostService;
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
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Body,Created,Updated,UpdateReason,BlogPostId,AuthorId")] Comment comment, int blogPostId)
        {
            BlogPost? blogPost = await _blogPostService.GetBlogPostAsync(blogPostId);
            ModelState.Remove("AuthorId");

            if (ModelState.IsValid)
            {

                comment.AuthorId = _userManager.GetUserId(User);

                if (User.Identity!.IsAuthenticated == true)
                {
                    // Automatically assign author and blogpost
                    comment.BlogPostId = blogPost!.Id;

                    comment.Created = DateTime.UtcNow;

                    await _blogPostService.AddCommentAsync(comment, blogPostId);

                    return RedirectToAction(blogPost.Slug, "Content");
                }
            }

            return View(comment);
        }

        // GET: Comments/Edit/5
        [AllowAnonymous]
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

            // TODO: Automatically assign author to Author ID & BlogPost and add if Statement to Delete

            if (comment.AuthorId == _userManager.GetUserId(User) || User.IsInRole("Admin") || User.IsInRole("Moderator"))
            {
                return View(comment);
            }

            return NotFound();
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AllowAnonymous]
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
                    // Automatically assign author to Author ID

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

            return View(comment);
        }

        // GET: Comments/Delete/5
        [AllowAnonymous]
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

            if (comment.AuthorId == _userManager.GetUserId(User) || User.IsInRole("Admin") || User.IsInRole("Moderator"))
            {
                return View(comment);
            }

            return NotFound();
        }

        // POST: Comments/Delete/5
        [AllowAnonymous]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _blogPostService.GetCommentAsync(id);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CommentExists(int id)
        {
            return (await _blogPostService.GetCommentsAsync()).Any(e => e.Id == id);
        }
    }
}
