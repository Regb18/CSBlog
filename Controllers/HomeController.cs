using CSBlog.Data;
using CSBlog.Models;
using CSBlog.Models.ViewModels;
using CSBlog.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly UserManager<BlogUser> _userManager;
        private readonly IEmailSender _emailService;

        public HomeController(ILogger<HomeController> logger, 
                              ApplicationDbContext context, 
                              IBlogPostService blogPostService,
                              UserManager<BlogUser> userManager,
                              IEmailSender emailService)
        {
            _logger = logger;
            _context = context;
            _blogPostService = blogPostService;
            _userManager = userManager;
            _emailService = emailService;
        }
        
        
        #region Index, Popular/Recent Posts, Search
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

        #endregion

        #region Privacy & Error
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion

        #region ContactMe

        // GET: EmailContact
        public IActionResult ContactMe()
        {

            // Instantiate EmailData
            EmailData emailData = new EmailData()
            {
                EmailAddress = "reginald.ac.barnes@gmail.com",
                FirstName = "Reggie",
                LastName = "Barnes",
            };


            return View(emailData);
        }

        // POST: EmailContact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactMe(EmailData emailData)
        {
            // IsValid = All required fields are checked
            if (ModelState.IsValid)
            {
                //string? swalMessage = string.Empty;

                try
                {
                    await _emailService.SendEmailAsync(emailData!.EmailAddress!,
                                                       emailData.EmailSubject!,
                                                       emailData.EmailBody!);

                    //swalMessage = "Your Email Has been Sent";

                    return RedirectToAction(nameof(Index));
                    // new is a route value that sends a parameter 
                }
                catch (Exception)
                {
                    //swalMessage = "Error: Email Send Failed";
                    return RedirectToAction(nameof(PopularPosts));
                    throw;
                }
            }


            return View(emailData);
        }

        #endregion
    }
}