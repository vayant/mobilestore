using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mobilestore.Models;
using Microsoft.AspNetCore.Authorization;
using mobilestore.Data;
using Microsoft.EntityFrameworkCore;

namespace mobilestore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return RedirectToAction("Index");

            var searchResults = _context.Products
                .Where(p => p.Name.ToLower().Contains(query.ToLower()) || 
                           p.Description.ToLower().Contains(query.ToLower()))
                .ToList();

            ViewBag.SearchTerm = query;
            return View("Search", searchResults);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult UserGuide()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
