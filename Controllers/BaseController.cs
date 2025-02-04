using Microsoft.AspNetCore.Mvc;
using mobilestore.Data;

namespace mobilestore.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
} 