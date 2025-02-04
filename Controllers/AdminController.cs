using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using mobilestore.Data;
using mobilestore.Models;
using mobilestore.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace mobilestore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
            : base(context)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> Orders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.OrderDate)
                    .Select(o => new OrderViewModel
                    {
                        Id = o.Id,
                        UserEmail = o.User.Email,
                        OrderDate = o.OrderDate,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status,
                        Items = o.OrderItems.Select(oi => new OrderItemViewModel
                        {
                            ProductName = oi.Product.Name,
                            Quantity = oi.Quantity,
                            Price = oi.Price
                        }).ToList()
                    })
                    .ToListAsync();

                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading orders");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBalance(int id, decimal balance)
        {
            try 
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    user.Balance = balance;
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Пользователь не найден" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating balance for user {UserId}", id);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
} 