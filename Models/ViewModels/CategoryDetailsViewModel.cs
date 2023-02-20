using X.PagedList;

namespace CSBlog.Models.ViewModels
{
    public class CategoryDetailsViewModel
    {
        public Category? Category { get; set; }
        public IPagedList<BlogPost>? Posts { get; set; }
    }
}
