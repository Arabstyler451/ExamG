using GreenfieldLocalHubWebApp.Data;
using GreenfieldLocalHubWebApp.Models;
using Microsoft.AspNetCore.Authorization;
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

            // Load Active Loyalty Offers (only truly active ones)
            var loyaltyAccount = await _context.loyaltyAccount
                .FirstOrDefaultAsync(l => l.UserId == userId);

            var activeOffers = loyaltyAccount != null && !string.IsNullOrEmpty(loyaltyAccount.ActiveOffers)
                ? loyaltyAccount.ActiveOffers.Split(',').Select(s => s.Trim()).ToList()
                : new List<string>();

            var consumedOffers = loyaltyAccount != null && !string.IsNullOrEmpty(loyaltyAccount.ConsumedOffers)
                ? loyaltyAccount.ConsumedOffers.Split(',').Select(s => s.Trim()).ToList()
                : new List<string>();

            // Remove any consumed offers that might still be in ActiveOffers (safety cleanup)
            activeOffers.RemoveAll(o => consumedOffers.Contains(o));

            // === Calculate Loyalty Discount (only on user-selected PendingOffer) ===
            float loyaltyDiscount = 0f;
            var pendingOffer = loyaltyAccount?.PendingOffer;

            // Guard: if the pending offer is no longer in ActiveOffers, clear it
            if (!string.IsNullOrEmpty(pendingOffer) && !activeOffers.Contains(pendingOffer))
            {
                loyaltyAccount.PendingOffer = null;
                pendingOffer = null;
                _context.Update(loyaltyAccount);
                await _context.SaveChangesAsync();
            }

            if (!string.IsNullOrEmpty(pendingOffer))
            {
                switch (pendingOffer)
                {
                    case "10% off Fruits & Vegetables":
                        foreach (var item in shoppingCartItems)
                        {
                            if (item.products.categories != null &&
                                string.Equals(item.products.categories.categoryName?.Trim(),
                                    "Fruit & Veg", StringComparison.OrdinalIgnoreCase))
                            {
                                loyaltyDiscount += item.products.productPrice * item.quantity * 0.10f;
                            }
                        }
                        break;

                    case "Free Cheese":
                        foreach (var item in shoppingCartItems)
                        {
                            if (item.products.productName?.Contains("Cheese",
                                    StringComparison.OrdinalIgnoreCase) == true)
                            {
                                loyaltyDiscount += item.products.productPrice * item.quantity;
                            }
                        }
                        break;

                    case "£5 Voucher":
                        if (subTotalAmount >= 20f)
                            loyaltyDiscount += 5f;
                        break;
                }
            }

            float total = subTotalAmount - loyaltyDiscount;

            ViewBag.subTotalAmount = subTotalAmount;
            ViewBag.loyaltyDiscount = loyaltyDiscount;
            ViewBag.total = total;
            ViewBag.ActiveOffers = activeOffers;
            ViewBag.PendingOffer = pendingOffer;

            return View(shoppingCartItems);
        }

        // GET: shoppingCarts/Details/5
        [Authorize(Roles = "Admin, Developer")]
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
        [Authorize(Roles = "Admin, Developer")]
        public IActionResult Create()
        {

            return View();
        }

        // POST: shoppingCarts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Developer")]
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
        [Authorize(Roles = "Admin, Developer")]
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
        [Authorize(Roles = "Admin, Developer")]
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
        [Authorize(Roles = "Admin, Developer")]
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
        [Authorize(Roles = "Admin, Developer")]
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


        
        //REORDER METHOD
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder(int orderId)
        {
            var orderItems = await _context.orderProducts
                .Where(op => op.ordersId == orderId)
                .ToListAsync();

            if (!orderItems.Any())
            {
                TempData["Error"] = "No items found for this order.";
                return RedirectToAction("Index", "Orders");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var cart = await _context.shoppingCart
                .FirstOrDefaultAsync(c => c.UserId == userId && c.shoppingCartStatus == true);

            if (cart == null)
            {
                cart = new shoppingCart
                {
                    UserId = userId,
                    shoppingCartStatus = true,
                    shoppingCartCreatedAt = DateTime.Now
                };

                _context.shoppingCart.Add(cart);
                await _context.SaveChangesAsync();
            }

            var productIds = orderItems.Select(i => i.productsId).ToList();

            var products = await _context.products
                .Where(p => productIds.Contains(p.productsId))
                .ToDictionaryAsync(p => p.productsId);

            var cartItems = await _context.shoppingCartItems
                .Where(ci => ci.shoppingCartId == cart.shoppingCartId)
                .ToListAsync();

            var outOfStockItems = new List<string>();

            foreach (var item in orderItems)
            {
                if (!products.TryGetValue(item.productsId, out var product) ||
                    product.stockQuantity < item.quantity)
                {
                    outOfStockItems.Add(product?.productName ?? "Unknown product");
                    continue;
                }

                var existingCartItem = cartItems
                    .FirstOrDefault(ci => ci.productsId == item.productsId);

                if (existingCartItem != null)
                {
                    var newQty = existingCartItem.quantity + item.quantity;
                    existingCartItem.quantity = Math.Min(newQty, product.stockQuantity);
                }
                else
                {
                    _context.shoppingCartItems.Add(new shoppingCartItems
                    {
                        shoppingCartId = cart.shoppingCartId,
                        productsId = item.productsId,
                        quantity = item.quantity,
                        unitPrice = item.unitPrice
                    });
                }
            }

            await _context.SaveChangesAsync();

            if (outOfStockItems.Any())
            {
                TempData["Warning"] = "Some items were out of stock: " + string.Join(", ", outOfStockItems);
            }
            else
            {
                TempData["Success"] = "Items added to your cart.";
            }

            return RedirectToAction("Index", "shoppingCarts");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyOffer(string selectedOffer)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var loyaltyAccount = await _context.loyaltyAccount
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (loyaltyAccount == null) return NotFound();

            // Validate the chosen offer is genuinely active before accepting it
            var activeOffers = string.IsNullOrEmpty(loyaltyAccount.ActiveOffers)
                ? new List<string>()
                : loyaltyAccount.ActiveOffers.Split(',').Select(s => s.Trim()).ToList();

            loyaltyAccount.PendingOffer = (selectedOffer == "none" || !activeOffers.Contains(selectedOffer))
                ? null
                : selectedOffer;

            _context.Update(loyaltyAccount);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // Controller method to display amount of items in the shopping cart
        public async Task<int> GetCartItemCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Adjusted to use ClaimTypes.NameIdentifier
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
