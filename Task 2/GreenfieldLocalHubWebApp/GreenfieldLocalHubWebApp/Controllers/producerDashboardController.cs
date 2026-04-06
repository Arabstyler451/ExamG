using GreenfieldLocalHubWebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GreenfieldLocalHubWebApp.Controllers
{
    [Authorize(Roles = "Producer")]
    public class producerDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public producerDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var producer = await _context.producers.FirstOrDefaultAsync(p => p.UserId == userId);

            if (producer == null)
            {
                return NotFound();
            }

            var products = await _context.products.Where(p => p.producersId == producer.producersId).ToListAsync();

            var orders = await _context.orders.Include(o => o.orderProducts).ThenInclude(op => op.products).Where(o => o.orderProducts.Any(op => op.products.producersId == producer.producersId)).ToListAsync();

            ViewBag.totalProducts = products.Count;
            ViewBag.lowStockProducts = products.Count(p => p.stockQuantity < 5);
            ViewBag.recentOrders = orders;

            return View(products);
        }
    }
}
