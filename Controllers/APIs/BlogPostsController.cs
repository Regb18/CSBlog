using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CSBlog.Data;
using CSBlog.Models;
using CSBlog.Services.Interfaces;

namespace CSBlog.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlogPostService _blogPostService;

        public BlogPostsController(ApplicationDbContext context, IBlogPostService blogPostService)
        {
            _context = context;
            _blogPostService = blogPostService;
        }

        // GET: api/BlogPosts
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPosts()
        //{
        //  if (_context.BlogPosts == null)
        //  {
        //      return NotFound();
        //  }
        //    return await _context.BlogPosts.ToListAsync();
        //}

        // GET: api/BlogPosts/GetRecentBlogPosts/8
        /// <summary>
        /// Returns a number of the most recent blogposts
        /// </summary>
        /// <param name="num">Number of Posts to retrieve</param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetRecentBlogPosts/{num}")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> BlogPosts(int? num)
        {
            if (num == null)
            {
                num = 4;
            }
            // Put the parentheses around await...value so it finishes finding the recent posts before being put in a list
            return (await _blogPostService.GetRecentPostsAsync(num.Value)).ToList();
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPost>> GetBlogPost(int id)
        {
          if (_context.BlogPosts == null)
          {
              return NotFound();
          }
            var blogPost = await _context.BlogPosts.FindAsync(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return blogPost;
        }

        // PUT: api/BlogPosts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogPost(int id, BlogPost blogPost)
        {
            if (id != blogPost.Id)
            {
                return BadRequest();
            }

            _context.Entry(blogPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BlogPosts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BlogPost>> PostBlogPost(BlogPost blogPost)
        {
          if (_context.BlogPosts == null)
          {
              return Problem("Entity set 'ApplicationDbContext.BlogPosts'  is null.");
          }
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlogPost", new { id = blogPost.Id }, blogPost);
        }

        // DELETE: api/BlogPosts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            if (_context.BlogPosts == null)
            {
                return NotFound();
            }
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogPostExists(int id)
        {
            return (_context.BlogPosts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
