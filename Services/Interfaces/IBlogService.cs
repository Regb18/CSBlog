using CSBlog.Models;
using Microsoft.EntityFrameworkCore;

namespace CSBlog.Services.Interfaces
{
    public interface IBlogService
    {
        // no return on these two Tasks
        public Task AddContactToCategoryAsync(int categoryId, int blogPostId);
        public Task AddBlogPostToTagsAsync(IEnumerable<int> tagIds, int blogPostId);

        // Returns the IEnumerable<Category>, appUserId is a string because that's what microsoft makes for IdentityUser
        public Task<IEnumerable<Tag>> GetAppUserCategoriesAsync(string BlogUserId);

        public Task<bool> IsBlogPostInTag(int tagId, int blogPostId);

        public Task RemoveAllBlogPostTagsAsync(int blogPostId);

        // Category Stuff - Testing
        public Task AddCategoryToContactsAsync(IEnumerable<int> blogPosts, int categoryId);

        public Task RemoveAllCategoryContactsAsync(int categoryId);
    }
}
