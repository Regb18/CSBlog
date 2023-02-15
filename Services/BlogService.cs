using CSBlog.Data;
using CSBlog.Models;
using CSBlog.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CSBlog.Services
{
    public class BlogService : IBlogService
    {
        private readonly ApplicationDbContext _context;

        // constructor is the same name as the class
        public BlogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddContactToCategoryAsync(int categoryId, int contactId)
        {
            throw new NotImplementedException();
        }
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

        // Returns the IEnumerable<Category>, appUserId is a string because that's what microsoft makes for IdentityUser
        public Task<IEnumerable<Tag>> GetAppUserCategoriesAsync(string BlogUserId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsBlogPostInTag(int tagId, int blogPostId)
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
                                         .Include(c => c.Tags)
                                         .FirstOrDefaultAsync(c => c.Id == blogPostId);

                // we can do this because we used an ICollection
                blogPost!.Tags.Clear();
                
                _context.Update(blogPost);
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {
                throw;
            }
        }

        // Category Stuff - Testing
        public Task AddCategoryToContactsAsync(IEnumerable<int> blogPosts, int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAllCategoryContactsAsync(int categoryId)
        {
            throw new NotImplementedException();
        }


    }
}
