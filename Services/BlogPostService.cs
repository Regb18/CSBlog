using Azure;
using CSBlog.Data;
using CSBlog.Helpers;
using CSBlog.Models;
using CSBlog.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CSBlog.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly ApplicationDbContext _context;

        public BlogPostService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region BlogPost CRUD Methods
        public async Task AddBlogPostAsync(BlogPost blogPost)
        {

            try
            {
                await _context.AddAsync(blogPost);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task UpdateBlogPostAsync(BlogPost blogPost)
        {
            try
            {
                _context.Update(blogPost);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteBlogPostAsync(BlogPost blogPost)
        {
            try
            {
                blogPost.IsDeleted = true;
                await UpdateBlogPostAsync(blogPost);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<BlogPost> GetBlogPostAsync(int blogPostId)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts
                                                   .Include(b => b.Category)
                                                   .Include(b => b.Tags)
                                                   .Include(b => b.Comments)
                                                   .FirstOrDefaultAsync(b => b.Id == blogPostId);

                return blogPost!;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BlogPost> GetBlogPostAsync(string slug)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts
                                                   .Include(b => b.Category)
                                                   .Include(b => b.Tags)
                                                   .Include(b => b.Comments)
                                                   .FirstOrDefaultAsync(b => b.Slug == slug);

                return blogPost!;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion


        #region Get BlogPost Methods
        public async Task<IEnumerable<BlogPost>> GetBlogPostsAsync()
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Include(c => c.Category)
                                                                .Include(c => c.Tags)
                                                                .Include(c => c.Comments)
                                                                .ToListAsync();

                return blogPosts;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IEnumerable<BlogPost>> GetPopularPostsAsync()
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Where(b => b.IsPublished == true && b.IsDeleted == false)
                                                                .Include(b => b.Category)
                                                                .Include(b => b.Tags)
                                                                .Include(b => b.Comments)
                                                                    .ThenInclude(c => c.Author)
                                                                .ToListAsync();

                return blogPosts.OrderByDescending(b => b.Comments.Count);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Gives a specific number of Popular posts based on count 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public async Task<IEnumerable<BlogPost>> GetPopularPostsAsync(int count)
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Where(b => b.IsPublished == true && b.IsDeleted == false)
                                                                .Include(b => b.Category)
                                                                .Include(b => b.Tags)
                                                                .Include(b => b.Comments)
                                                                    .ThenInclude(c => c.Author)
                                                                .ToListAsync();

                return blogPosts.OrderByDescending(b => b.Comments.Count).Take(count);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<BlogPost>> GetRecentPostsAsync()
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Where(b => b.IsPublished == true && b.IsDeleted == false)
                                                                .Include(b => b.Category)
                                                                .Include(b => b.Tags)
                                                                .Include(b => b.Comments)
                                                                    .ThenInclude(c => c.Author)
                                                                .ToListAsync();

                return blogPosts.OrderByDescending(b => b.Created);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count)
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Where(b => b.IsPublished == true && b.IsDeleted == false)
                                                                .Include(b => b.Category)
                                                                .Include(b => b.Tags)
                                                                .Include(b => b.Comments)
                                                                    .ThenInclude(c => c.Author)
                                                                .ToListAsync();

                return blogPosts.OrderByDescending(b => b.Created).Take(count);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<BlogPost>> GetCategoryPostsAsync(int? categoryId)
        {
            try
            {
                IEnumerable<BlogPost> blogPosts = await _context.BlogPosts
                                                                .Where(b => b.IsPublished == true && b.IsDeleted == false)
                                                                .Where(b => b.CategoryId == categoryId)
                                                                .Include(b => b.Category)
                                                                .Include(b => b.Tags)
                                                                .Include(b => b.Comments)
                                                                    .ThenInclude(c => c.Author)
                                                                .ToListAsync();

                return blogPosts.OrderByDescending(b => b.Created);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion


        #region Category CRUD Methods
        public async Task AddCategoryAsync(Category category)
        {
            try
            {
                _context.Add(category);
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task DeleteCategoryAsync(Category category)
        {
            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<Category> GetCategoryAsync(int categoryId)
        {
            try
            {
                Category? category = await _context.Categories
                                                   .Include(c => c.BlogPosts)
                                                   .FirstOrDefaultAsync(m => m.Id == categoryId);
                return category!;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task UpdateCategoryAsync(Category category)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            try
            {
                IEnumerable<Category> categories = await _context.Categories
                                                      .Include(c => c.BlogPosts)
                                                      .ToListAsync();

                return categories;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion


        #region Tag CRUD Methods
        public async Task AddTagAsync(Tag tag)
        {
            try
            {
                _context.Add(tag);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IEnumerable<Tag>> GetTagsAsync()
        {
            try
            {
                IEnumerable<Tag> tags = await _context.Tags
                                                      .Include(c => c.BlogPosts)
                                                      .ToListAsync();

                return tags;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task DeleteTagAsync(Tag tag)
        {
            try
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task UpdateTagAsync(Tag tag)
        {
            _context.Update(tag);
            await _context.SaveChangesAsync();
        }
        public async Task<Tag> GetTagAsync(int tagId)
        {
            try
            {
                Tag? tag = await _context.Tags
                                         .Include(c => c.BlogPosts)
                                            .ThenInclude(b=>b.Category)
                                         .FirstOrDefaultAsync(m => m.Id == tagId);
                return tag!;

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion


        #region Comment CRUD Methods
        public async Task AddCommentAsync(Comment comment, int blogPostId)
        {
            BlogPost? blogPost = await _context.BlogPosts
                                 .Include(c => c.Comments) // Eager Load
                                 .FirstOrDefaultAsync(c => c.Id == blogPostId);

            try
            {
                _context.Add(comment);
                blogPost!.Comments.Add(comment);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task UpdateCommentAsync(Comment comment)
        {
            _context.Update(comment);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Comment>> GetCommentsAsync()
        {
            try
            {
                IEnumerable<Comment> comments = await _context.Comments
                                                              .Include(c => c.Author)
                                                              .Include(c => c.BlogPost)
                                                              .ToListAsync();

                return comments;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<IEnumerable<Comment>> GetRecentCommentsAsync(int blogPostId)
        {
            try
            {
                IEnumerable<Comment> comments = await _context.Comments
                                                              .Where(c=>c.BlogPostId == blogPostId)
                                                              .Include(c => c.Author)
                                                              .Include(c => c.BlogPost)
                                                              .ToListAsync();

                return comments.OrderByDescending(b => b.Created);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task DeleteCommentAsync(Comment comment)
        {
            try
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<Comment> GetCommentAsync(int commentId)
        {
            try
            {
                Comment? comment = await _context.Comments
                                         .Include(c => c.BlogPost)
                                         .Include(c => c.Author)
                                         .FirstOrDefaultAsync(m => m.Id == commentId);
                return comment!;

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion


        #region Add Tags / Search / Slug
        public async Task AddBlogPostToTagsAsync(IEnumerable<int> tagIds, int blogPostId)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts
                                                 .Include(c => c.Tags) // Eager Load
                                                 .FirstOrDefaultAsync(c => c.Id == blogPostId);

                foreach (int tagId in tagIds)
                {
                    Tag? tag = await _context.Tags.FindAsync(tagId);

                    if (blogPost != null && tag != null)
                    {
                        // Can use add because we're working with objects
                        blogPost.Tags.Add(tag);
                    }
                }

                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task AddBlogPostToTagsAsync(string stringTags, int blogPostId)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts.FindAsync(blogPostId);

                if (blogPost == null)
                {
                    return;
                }

                foreach (string tagName in stringTags.Split(","))
                {
                    Tag? tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name.Trim().ToLower() == tagName.Trim().ToLower());

                    if (tag != null)
                    {
                        blogPost.Tags.Add(tag);
                    }
                    else
                    {
                        Tag newTag = new Tag() { Name = tagName.Trim() };
                        // Database knows this is a Tag so it will add it to Tags
                        // Want to add this to the database before adding it to the blogpost
                        _context.Add(newTag);

                        blogPost.Tags.Add(newTag);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<bool> IsTagOnBlogPostAsync(int tagId, int blogPostId)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts
                                                 .Include(c => c.Tags) // Eager Load
                                                 .FirstOrDefaultAsync(c => c.Id == blogPostId);

                bool isInTag = blogPost!.Tags.Select(c => c.Id).Contains(tagId);

                return isInTag;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task RemoveAllBlogPostTagsAsync(int blogPostId)
        {
            try
            {
                // c represents an individual contact record in the database
                BlogPost? blogPost = await _context.BlogPosts
                                         .Include(b => b.Tags)
                                         .FirstOrDefaultAsync(b => b.Id == blogPostId);
                if (blogPost != null)
                {
                    // we can do this because we used an ICollection
                    blogPost!.Tags.Clear();

                    _context.Update(blogPost);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<BlogPost> SearchBlogPosts(string? searchString)
        {
            try
            {
                // IEnumerable<BlogPost> blogPosts = new(); - can't instantiate an interface
                IEnumerable<BlogPost> blogPosts = new List<BlogPost>();

                if (string.IsNullOrEmpty(searchString))
                {
                    return blogPosts;
                }
                else
                {
                    searchString = searchString.Trim().ToLower();

                    blogPosts = _context.BlogPosts.Where(b => b.Title!.ToLower().Contains(searchString) ||
                                                         b.Abstract!.ToLower().Contains(searchString) ||
                                                         b.Content!.ToLower().Contains(searchString) ||
                                                         b.Category!.Name!.ToLower().Contains(searchString) ||
                                                         //.Any can search all the properties of the collection
                                                         b.Comments.Any(c => c.Body!.ToLower().Contains(searchString) ||
                                                                        c.Author!.FirstName!.ToLower().Contains(searchString) ||
                                                                        c.Author!.LastName!.ToLower().Contains(searchString)) ||
                                                         b.Tags.Any(c => c.Name!.ToLower().Contains(searchString)))
                                                  .Include(b => b.Comments)
                                                        .ThenInclude(c => c.Author)
                                                  .Include(b => b.Category)
                                                  .Include(b => b.Tags)
                                                  .Where(b => b.IsPublished == true && b.IsDeleted == false)
                                                  .AsNoTracking()
                                                  .OrderByDescending(b => b.Created)
                                                  .AsEnumerable();

                    return blogPosts;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> ValidateSlugAsync(string title, int blogPostId)
        {
            try
            {
                string newSlug = StringHelper.BlogSlug(title);

                // a new blogPost has no ID yet so it should equal 0. If it does we assign it a new slug
                // if it doesn't, goes to the else 
                if (blogPostId == 0)
                {
                    // AnyAsync - checking the database to see if theres a matching slug
                    return !await _context.BlogPosts.AnyAsync(b => b.Slug == newSlug);
                }
                else
                {
                    // ORM (entity framework for us) tracks the data, AsNoTracking stops ORM from tracking, we just need the values
                    // Entity framework stops us from finding and accessing an instantiation with something ekse
                    BlogPost? blogPost = await _context.BlogPosts.AsNoTracking().FirstOrDefaultAsync(b => b.Id == blogPostId);

                    string? oldSlug = blogPost?.Slug;

                    // another way to compare strings 
                    if (!string.Equals(oldSlug, newSlug))
                    {
                        return !await _context.BlogPosts.AnyAsync(b => b.Id != blogPost!.Id && b.Slug == newSlug);
                    }
                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

    }
}


