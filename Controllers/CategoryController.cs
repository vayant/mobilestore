using Microsoft.AspNetCore.Mvc;
using mobilestore.Data;
using Microsoft.EntityFrameworkCore;

namespace mobilestore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = new List<string>
            {
                "Apple",
                "Samsung",
                "Xiaomi",
                "Google",
                "OnePlus",
                "Nothing"
            };
            return View(categories);
        }

        public async Task<IActionResult> Products(string category)
        {
            var products = await _context.Products
                .Where(p => p.Category == category)
                .ToListAsync();

            ViewBag.CategoryName = category;
            return View(products);
        }
    }
} 