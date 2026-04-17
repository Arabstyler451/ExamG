using GreenfieldLocalHubWebApp.Data;
using GreenfieldLocalHubWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GreenfieldLocalHubWebApp.Controllers
{
    public class shoppingCartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public shoppingCartsController(ApplicationDbContext context)
        {
            _context = context;
        }




        // GET: shoppingCarts
        public async Task<IActionResult> Index()
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Get or create active shopping cart
            var shoppingCart = await _context.shoppingCart
                .FirstOrDefaultAsync(c => c.UserId == userId && c.shoppingCartStatus);

            if (shoppingCart == null)
            {
                shoppingCart = new shoppingCart
                {
                    UserId = userId,
                    shoppingCartCreatedAt = DateTime.Now,
                    shoppingCartStatus = true
                };
                _context.shoppingCart.Add(shoppingCart);
                await _context.SaveChangesAsync();
            }

            // Load cart items with product details + categories
            var shoppingCartItems = await _context.shoppingCartItems
                .Where(sci => sci.shoppingCartId == shoppingCart.shoppingCartId)
                .Include(sci => sci.shoppingCart)
                .Include(sci => sci.products)
                    .ThenInclude(p => p.categories)           // Important: load categories
                .ToListAsync();

            // Calculate subtotal
            float subTotalAmount = shoppingCartItems.Sum(item =>
                item.products.productPrice * item.quantity);

            // Load Active Loyalty Offers
            var loyaltyAccount = await _context.loyaltyAccount
                .FirstOrDefaultAsync(l => l.UserId == userId);

            var activeOffers = loyaltyAccount != null && !string.IsNullOrEmpty(loyaltyAccount.ActiveOffers)
                ? loyaltyAccount.ActiveOffers.Split(',').ToList()
                : new List<string>();

            // === Calculate Loyalty Discounts ===
            float loyaltyDiscount = 0f;

            foreach (var item in shoppingCartItems)
            {
                var price = item.products.productPrice * item.quantity;

                // 10% off Fruits & Vegetables - ONLY on "Fruit & Veg" category
                if (activeOffers.Contains("10% off Fruits & Vegetables") &&
                    item.products.categories != null && string.Equals(item.products.categories.categoryName?.Trim(), "Fruit & Veg", StringComparison.OrdinalIgnoreCase))
                {
                    loyaltyDiscount += (float)(price * 0.10);
                }

                // Free Cheese - make cheese items free
                if (activeOffers.Contains("Free Cheese") &&
                    item.products.productName?.Contains("Cheese", StringComparison.OrdinalIgnoreCase) == true)
                {
                    loyaltyDiscount += (float)price;   // this item becomes free
                }
            }

            // £5 Voucher - Only if order is £20 or more
            if (activeOffers.Contains("£5 Voucher") && subTotalAmount >= 20f)
            {
                loyaltyDiscount += 5f;
            }

            // Free Delivery is better handled in checkout (shipping), not here

            // Keep your old order-based 10% discount (if you still want both)
            var orderCount = await _context.orders.CountAsync(oc => oc.UserId == userId);
            if (orderCount >= 5)
            {
                loyaltyDiscount += (float)(subTotalAmount * 0.10f);
            }

            float total = subTotalAmount - loyaltyDiscount;

            // Pass data to view
            ViewBag.subTotalAmount = subTotalAmount;
            ViewBag.loyaltyDiscount = loyaltyDiscount;
            ViewBag.total = total;
            ViewBag.orderCount = orderCount;
            ViewBag.ActiveOffers = activeOffers;

            return View(shoppingCartItems);
        }

        // GET: shoppingCarts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            if (id == null)
            {
                return NotFound();
            }

            var shoppingCart = await _context.shoppingCart
                .FirstOrDefaultAsync(m => m.shoppingCartId == id);
            if (shoppingCart == null)
            {
                return NotFound();
            }

            return View(shoppingCart);
        }


        // GET: shoppingCarts/Create
        public IActionResult Create()
        {

            return View();
        }

        // POST: shoppingCarts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("shoppingCartId,UserId,shoppingCartCreatedAt,shoppingCartStatus")] shoppingCart shoppingCart)
        {

            if (ModelState.IsValid)
            {
                _context.Add(shoppingCart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shoppingCart);
        }

        // GET: shoppingCarts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            if (id == null)
            {
                return NotFound();
            }

            var shoppingCart = await _context.shoppingCart.FindAsync(id);
            if (shoppingCart == null)
            {
                return NotFound();
            }
            return View(shoppingCart);
        }

        // POST: shoppingCarts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("shoppingCartId,UserId,shoppingCartCreatedAt,shoppingCartStatus")] shoppingCart shoppingCart)
        {
            if (id != shoppingCart.shoppingCartId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoppingCart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!shoppingCartExists(shoppingCart.shoppingCartId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(shoppingCart);
        }

        // GET: shoppingCarts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCart = await _context.shoppingCart
                .FirstOrDefaultAsync(m => m.shoppingCartId == id);
            if (shoppingCart == null)
            {
                return NotFound();
            }

            return View(shoppingCart);
        }

        // POST: shoppingCarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoppingCart = await _context.shoppingCart.FindAsync(id);
            if (shoppingCart != null)
            {
                _context.shoppingCart.Remove(shoppingCart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool shoppingCartExists(int id)
        {
            return _context.shoppingCart.Any(e => e.shoppingCartId == id);
        }



        // Controller method to display amount of items in the shopping cart
        public async Task<int> GetCartItemCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return 0;

            var shoppingCart = await _context.shoppingCart
                .FirstOrDefaultAsync(c => c.UserId == userId && c.shoppingCartStatus);

            if (shoppingCart == null) return 0;

            // Sum the quantity column to get total number of items in the shopping cart
            var totalItems = await _context.shoppingCartItems
                .Where(sci => sci.shoppingCartId == shoppingCart.shoppingCartId)
                .SumAsync(sci => sci.quantity);

            return totalItems;
        }
    }
}
