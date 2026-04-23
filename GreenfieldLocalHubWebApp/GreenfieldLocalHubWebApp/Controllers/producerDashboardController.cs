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


        public async Task<IActionResult> Index(string activeTab = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var producer = await _context.producers
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (producer == null) return NotFound();

            var products = await _context.products
                .Where(p => p.producersId == producer.producersId)
                .ToListAsync();

            var orders = await _context.orders
                .Include(o => o.orderProducts)
                    .ThenInclude(op => op.products)
                .Where(o => o.orderProducts
                    .Any(op => op.products.producersId == producer.producersId))
                .ToListAsync();

            // Load emails for each order's user from AspNetUsers
            var userIds = orders.Select(o => o.UserId).Distinct().ToList();
            var users = await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Email);

            ViewBag.totalProducts = products.Count;
            ViewBag.lowStockProducts = products.Count(p => p.stockQuantity < 5);
            ViewBag.totalStockUnits = products.Sum(p => p.stockQuantity);
            ViewBag.recentOrders = orders;
            ViewBag.userEmails = users;
            ViewBag.activeTab = activeTab ?? "products";

            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var producer = await _context.producers
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (producer == null) return NotFound();

            var order = await _context.orders
                .Include(o => o.orderProducts)
                    .ThenInclude(op => op.products)
                .FirstOrDefaultAsync(o => o.ordersId == orderId &&
                    o.orderProducts.Any(op => op.products.producersId == producer.producersId));

            if (order == null) return NotFound();

            // Different status flow for collection vs delivery
            order.orderStatus = order.collection
                ? order.orderStatus switch
                {
                    "Pending" => "Processing",
                    "Processing" => "Ready to Collect",
                    "Ready to Collect" => "Collected",
                    _ => order.orderStatus
                }
                : order.orderStatus switch
                {
                    "Pending" => "Processing",
                    "Processing" => "Dispatched",
                    "Dispatched" => "Delivered",
                    _ => order.orderStatus
                };

            _context.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { activeTab = "orders" });
        }
    }
}
