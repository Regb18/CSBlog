using CSBlog.Data;
using CSBlog.Models;
using CSBlog.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;
// using directives are namespaces, can using anything in there

namespace CSBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBlogPostService _blogPostService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IBlogPostService blogPostService)
        {
            _logger = logger;
            _context = context;
            _blogPostService = blogPostService;
        }

        public async Task<IActionResult> Index(int? pageNum)
        {
            int pageSize = 4;
            // if its trying to show a page that doesn't exist it gives the 1st one
            int page = pageNum ?? 1;

                                            // the parentheses makes sure this happens first before anything else happens
            IPagedList<BlogPost> blogPosts = (await _blogPostService.GetRecentPostsAsync()).ToPagedList(page, pageSize);

            return View(blogPosts);
        }

        public async Task<IActionResult> PopularPosts(int? pageNum)
        {
            int pageSize = 4;
            // if its trying to show a page that doesn't exist it gives the 1st one
            int page = pageNum ?? 1;

            // the parentheses makes sure this happens first before anything else happens
            IPagedList<BlogPost> blogPosts = (await _blogPostService.GetPopularPostsAsync()).ToPagedList(page, pageSize);

            return View(blogPosts);
        }

        public async Task<IActionResult> RecentPosts(int? pageNum)
        {
            int pageSize = 4;
            // if its trying to show a page that doesn't exist it gives the 1st one
            int page = pageNum ?? 1;

            // the parentheses makes sure this happens first before anything else happens
            IPagedList<BlogPost> blogPosts = (await _blogPostService.GetRecentPostsAsync()).ToPagedList(page, pageSize);

            return View(blogPosts);
        }

        public IActionResult SearchIndex(int? pageNum, string? searchString) 
        {
            int pageSize = 3;
            int page = pageNum ?? 1;

            IPagedList<BlogPost> blogPosts = (_blogPostService.SearchBlogPosts(searchString)).ToPagedList(page, pageSize);


            return View(nameof(Index), blogPosts);
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}