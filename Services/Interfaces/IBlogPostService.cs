using CSBlog.Models;

namespace CSBlog.Services.Interfaces
{
    public interface IBlogPostService
    {
        // CRUD Services

        #region BlogPost CRUD Methods
        public Task AddBlogPostAsync(BlogPost blogPost);
        public Task UpdateBlogPostAsync(BlogPost blogPost);
        public Task<BlogPost> GetBlogPostAsync(int blogPostId);
        public Task DeleteBlogPostAsync(BlogPost blogPost);
        #endregion

        #region Get BlogPosts Methods
        // GetBlogPostsAsync()
        public Task<IEnumerable<BlogPost>> GetBlogPostsAsync();
        // GetPopularPostsAsync()
        public Task<IEnumerable<BlogPost>> GetPopularPostsAsync();
        public Task<IEnumerable<BlogPost>> GetPopularPostsAsync(int count);
        // GetRecentPostsAsync()
        public Task<IEnumerable<BlogPost>> GetRecentPostsAsync();
        public Task<IEnumerable<BlogPost>> GetRecentPostsAsync(int count);
        #endregion


        #region Category CRUD Methods
        public Task AddCategoryAsync(Category category);
        public Task UpdateCategoryAsync(Category category);
        public Task<Category> GetCategoryAsync(int categoryId);
        public Task DeleteCategoryAsync(Category category);

        public Task<IEnumerable<Category>> GetCategoriesAsync();
        #endregion

        #region Tag CRUD Methods
        public Task AddTagAsync(Tag tag);
        public Task UpdateTagAsync(Tag tag);
        public Task<IEnumerable<Tag>> GetTagsAsync();
        public Task DeleteTagAsync(Tag tag);
        public Task<Tag> GetTagAsync(int tagId);
        #endregion

        #region Comment CRUD Methods
        public Task AddCommentAsync(Comment comment);
        public Task UpdateCommentAsync(Comment comment);
        public Task<IEnumerable<Comment>> GetCommentsAsync();
        public Task DeleteCommentAsync(Comment comment);
        public Task<Comment> GetCommentAsync(int commentId);
        #endregion

        #region Add Tags / Search / Slug
        public Task<bool> IsTagOnBlogPostAsync(int tagId, int BlogPostId);
        public Task AddBlogPostToTagsAsync(IEnumerable<int> tagIds, int blogPostId);
        public Task RemoveAllBlogPostTagsAsync(int blogPostId);
        public IEnumerable<BlogPost> Search(string searchString);
        public Task<bool> ValidateSlugAsync(string title, int blogPostId);
        #endregion


    }
}
