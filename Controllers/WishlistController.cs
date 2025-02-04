using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using mobilestore.Data;
using mobilestore.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace mobilestore.Controllers
{
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlistItems = await _context.Wishlist
                .Include(w => w.Product)
                .Where(w => w.UserId == userId)
                .ToListAsync();
            return View(wishlistItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody]int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { redirectUrl = "/Home/Login" });
            }
            
            var existingItem = await _context.Wishlist
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existingItem == null)
            {
                var wishlistItem = new WishlistClass
                {
                    UserId = userId,
                    ProductId = productId,
                    DateAdded = DateTime.Now
                };

                _context.Wishlist.Add(wishlistItem);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist([FromBody]int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlistItem = await _context.Wishlist
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (wishlistItem != null)
            {
                _context.Wishlist.Remove(wishlistItem);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> IsInWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var exists = await _context.Wishlist
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
            return Json(new { isInWishlist = exists });
        }
    }
} 