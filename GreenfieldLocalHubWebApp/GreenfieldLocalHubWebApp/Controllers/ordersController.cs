using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GreenfieldLocalHubWebApp.Data;
using GreenfieldLocalHubWebApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace GreenfieldLocalHubWebApp.Controllers
{
    public class ordersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ordersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: orders
        public async Task<IActionResult> Index()
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            // Get the currently logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }


            // If the user is an admin, show all orders
            if (User.IsInRole("Admin"))
            {
                var allOrders = await _context.orders.Include(o => o.orderProducts).ThenInclude(op => op.products).ToListAsync();

                ViewData["Layout"] = "_AccountLayout";
                return View(allOrders);
            }

            //If the user is a producer, show only orders that contain their products. Otherwise, show only the user's own orders.
            else if (User.IsInRole("Producer"))
            {
                var producerProducts = await _context.products.Where(p => p.producers.UserId == userId).Select(p => p.productsId).ToListAsync();
                var producerOrders = await _context.orderProducts.Where(op => producerProducts.Contains(op.productsId)).Include(op => op.orders).Include(op => op.products).ToListAsync();

                ViewData["Layout"] = "_AccountLayout";
                return View(producerOrders.Select(vo => vo.orders).Distinct().ToList());
            }
            else
            {
                var orders = await _context.orders.Where(o => o.UserId == userId).Include(o => o.orderProducts).ThenInclude(op => op.products).ToListAsync();

                ViewData["Layout"] = "_AccountLayout";
                return View(orders);
            }

        }


        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            if (id == null)
            {
                return NotFound();
            }

            var orderProducts = await _context.orderProducts
                .Where(op => op.ordersId == id)
                .Include(op => op.orders)
                .Include(op => op.products)
                .ToListAsync();

            if (!orderProducts.Any())
            {
                return NotFound();
            }

            return View(orderProducts);
        }

        // GET: orders/Create
        public async Task<IActionResult> Create(int shoppingCartId, int? selectedAddressId = null)
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewBag.shoppingCartId = shoppingCartId;

            // Get user's addresses
            var userAddresses = await _context.address.Where(a => a.UserId == userId).ToListAsync();

            ViewBag.HasAddresses = userAddresses.Any();

            // Build dropdown (pre-select the newly added address if coming back from Addresses/Create)
            ViewData["AddressId"] = new SelectList(userAddresses, "addressId", "street", selectedAddressId);
            return View();
        }

        // POST: orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ordersId,addressId,delivery,collection,deliveryType,orderCollectionDate")] orders orders, int shoppingCartId)
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                ViewBag.shoppingCartId = shoppingCartId;
                return View(orders);
            }

            orders.UserId = userId;
            ModelState.Remove("UserId");
            orders.orderDate = DateOnly.FromDateTime(DateTime.Today);
            ModelState.Remove("orderDate");
            orders.orderStatus = "Pending";
            ModelState.Remove("orderStatus");

            var shoppingCart = await _context.shoppingCart
                .FirstOrDefaultAsync(sc => sc.shoppingCartId == shoppingCartId && sc.UserId == userId && sc.shoppingCartStatus);

            if (shoppingCart == null)
                return NotFound();

            // Load shopping cart items + product + category (needed for vegetable discount)
            var shoppingCartItems = await _context.shoppingCartItems
                .Where(sci => sci.shoppingCartId == shoppingCartId)
                .Include(sci => sci.products)
                    .ThenInclude(p => p.categories)
                .ToListAsync();

            if (!shoppingCartItems.Any())
            {
                ModelState.AddModelError("", "Your shopping cart is empty.");
                ViewBag.shoppingCartId = shoppingCartId;
                return View(orders);
            }

            // === Calculate Subtotal & Loyalty Discounts ===
            float subTotal = shoppingCartItems.Sum(item => item.products.productPrice * item.quantity);

            // Load active offers
            var loyaltyAccount = await _context.loyaltyAccount
                .FirstOrDefaultAsync(l => l.UserId == userId);

            var activeOffers = loyaltyAccount != null && !string.IsNullOrEmpty(loyaltyAccount.ActiveOffers)
                ? loyaltyAccount.ActiveOffers.Split(',').ToList()
                : new List<string>();

            float loyaltyDiscount = 0f;
            var usedOffers = new List<string>(); // Track which offers were actually used

            foreach (var item in shoppingCartItems)
            {
                var price = item.products.productPrice * item.quantity;

                // 10% off Fruits & Vegetables
                if (activeOffers.Contains("10% off Fruits & Vegetables") &&
                    item.products.categories != null &&
                    string.Equals(item.products.categories.categoryName?.Trim(), "Fruit & Veg", StringComparison.OrdinalIgnoreCase))
                {
                    loyaltyDiscount += (float)(price * 0.10);
                    if (!usedOffers.Contains("10% off Fruits & Vegetables"))
                        usedOffers.Add("10% off Fruits & Vegetables");
                }

                // Free Cheese
                if (activeOffers.Contains("Free Cheese") &&
                    item.products.productName?.Contains("Cheese", StringComparison.OrdinalIgnoreCase) == true)
                {
                    loyaltyDiscount += (float)price;
                    if (!usedOffers.Contains("Free Cheese"))
                        usedOffers.Add("Free Cheese");
                }
            }

            // £5 Voucher - Only if order is £20 or more
            if (activeOffers.Contains("£5 Voucher") && subTotal >= 20f)
            {
                loyaltyDiscount += 5f;
                if (!usedOffers.Contains("£5 Voucher"))
                    usedOffers.Add("£5 Voucher");
            }

            // Order count discount (this is NOT a voucher, it's a permanent discount)
            var orderCount = await _context.orders.CountAsync(oc => oc.UserId == userId);
            if (orderCount >= 5)
            {
                loyaltyDiscount += (float)(subTotal * 0.10f);
            }

            orders.totalAmount = subTotal - loyaltyDiscount;
            ModelState.Remove("totalAmount");

            // Address handling
            if (orders.addressId.HasValue)
            {
                var selectedAddress = await _context.address.FindAsync(orders.addressId.Value);
                if (selectedAddress != null)
                {
                    orders.DeliveryStreet = selectedAddress.street;
                    orders.DeliveryCity = selectedAddress.city;
                    orders.DeliveryPostalCode = selectedAddress.postalCode;
                    orders.DeliveryCountry = selectedAddress.country;
                }
            }

            // Delivery / Collection validation
            if (!orders.collection && !orders.delivery)
                ModelState.AddModelError("delivery", "Please select either delivery or collection for your order");

            if (orders.collection)
            {
                ModelState.Remove("deliveryType");
                if (orders.orderCollectionDate == null)
                    ModelState.AddModelError("orderCollectionDate", "Collection date is required.");
                else if (orders.orderCollectionDate.Value < DateOnly.FromDateTime(DateTime.Today.AddDays(2)))
                    ModelState.AddModelError("orderCollectionDate", $"Collection date must be at least 2 days from today.");
            }
            if (orders.delivery && string.IsNullOrWhiteSpace(orders.deliveryType))
                ModelState.AddModelError("deliveryType", "Please select a delivery type for your order");

            if (!ModelState.IsValid)
            {
                ViewBag.shoppingCartId = shoppingCartId;
                return View(orders);
            }

            // ====================== SAVE ORDER ======================
            _context.orders.Add(orders);
            await _context.SaveChangesAsync();

            // Create orderProducts and update stock
            foreach (var shoppingCartItem in shoppingCartItems)
            {
                if (shoppingCartItem.products.stockQuantity < shoppingCartItem.quantity)
                {
                    ModelState.AddModelError("", $"Sorry, we only have {shoppingCartItem.products.stockQuantity} units of {shoppingCartItem.products.productName} in stock.");
                    ViewBag.shoppingCartId = shoppingCartId;
                    return View(orders);
                }

                var orderProduct = new orderProducts
                {
                    ordersId = orders.ordersId,
                    productsId = shoppingCartItem.productsId,
                    quantity = shoppingCartItem.quantity,
                    unitPrice = shoppingCartItem.unitPrice
                };
                _context.orderProducts.Add(orderProduct);

                shoppingCartItem.products.stockQuantity -= shoppingCartItem.quantity;
            }

            shoppingCart.shoppingCartStatus = false;
            await _context.SaveChangesAsync();

            // ====================== CONSUME ONLY THE OFFERS THAT WERE USED ======================
            if (usedOffers.Any())
            {
                // Get a FRESH instance from database to avoid tracking issues
                var loyaltyAccountForOffers = await _context.loyaltyAccount
                    .AsNoTracking()  // Add this to ensure fresh data
                    .FirstOrDefaultAsync(l => l.UserId == userId);

                if (loyaltyAccountForOffers != null && !string.IsNullOrEmpty(loyaltyAccountForOffers.ActiveOffers))
                {
                    var activeList = loyaltyAccountForOffers.ActiveOffers.Split(',').ToList();
                    bool changed = false;

                    foreach (var offer in usedOffers)
                    {
                        if (activeList.Contains(offer))  // Use Contains instead of Remove in loop
                        {
                            activeList.Remove(offer);
                            changed = true;
                            Console.WriteLine($"Removing offer: {offer}");
                        }
                    }

                    if (changed)
                    {
                        var updatedAccount = await _context.loyaltyAccount
                            .FirstOrDefaultAsync(l => l.UserId == userId);

                        if (updatedAccount != null)
                        {
                            updatedAccount.ActiveOffers = string.Join(",", activeList.Where(s => !string.IsNullOrEmpty(s)));
                            _context.Entry(updatedAccount).State = EntityState.Modified;
                            await _context.SaveChangesAsync();

                            // Verify the removal
                            var verifyAccount = await _context.loyaltyAccount
                                .FirstOrDefaultAsync(l => l.UserId == userId);
                            Console.WriteLine($"VERIFICATION - ActiveOffers after removal: {verifyAccount?.ActiveOffers ?? "NULL"}");
                        }
                    }
                }
            }

            // ====================== AWARD LOYALTY POINTS ======================
            try
            {
                var loyaltyAccountForPoints = await _context.loyaltyAccount
                    .FirstOrDefaultAsync(l => l.UserId == userId);

                if (loyaltyAccountForPoints == null)
                {
                    loyaltyAccountForPoints = new loyaltyAccount
                    {
                        UserId = userId,
                        pointsBalance = 0,
                        loyaltyTier = "Bronze"
                    };
                    _context.loyaltyAccount.Add(loyaltyAccountForPoints);
                    await _context.SaveChangesAsync();
                }

                // Store old tier before updating
                var oldTier = loyaltyAccountForPoints.loyaltyTier;

                int pointsEarned = (int)(orders.totalAmount * 10); // 10 points per £1
                var transaction = new loyaltyTransaction
                {
                    loyaltyAccountId = loyaltyAccountForPoints.loyaltyAccountId,
                    ordersId = orders.ordersId,
                    loyaltyPoints = pointsEarned,
                    transactionType = "Earn",
                    transactionDate = DateTime.Now
                };

                _context.loyaltyTransaction.Add(transaction);
                loyaltyAccountForPoints.pointsBalance += pointsEarned;

                loyaltyAccountForPoints.loyaltyTier = loyaltyAccountForPoints.pointsBalance switch
                {
                    >= 5000 => "Platinum",
                    >= 2000 => "Gold",
                    >= 500 => "Silver",
                    _ => "Bronze"
                };

                await _context.SaveChangesAsync();

                // Grant tier-based offers if tier improved
                if (oldTier != loyaltyAccountForPoints.loyaltyTier)
                {
                    // You'll need to add a reference to your loyaltyAccountsController or move this logic
                    await GrantTierOffersForOrder(userId, loyaltyAccountForPoints.loyaltyTier);
                }

                TempData["LoyaltyMessage"] = $"You earned {pointsEarned} loyalty points!";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Loyalty error: {ex.Message}");
            }

            return RedirectToAction("Index", "shoppingCarts");
        }





        // GET: orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            ViewData["addressId"] = new SelectList(_context.address, "addressId", "addressId", orders.addressId);
            return View(orders);
        }



        // POST: orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ordersId,addressId,UserId,totalAmount,delivery,collection,deliveryType,orderStatus,orderCollectionDate,orderDate,DeliveryStreet,DeliveryCity,DeliveryPostalCode,DeliveryCountry")] orders orders)
        {
            if (id != orders.ordersId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ordersExists(orders.ordersId))
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
            ViewData["addressId"] = new SelectList(_context.address, "addressId", "addressId", orders.addressId);
            return View(orders);
        }

        // GET: orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.orders
                .Include(o => o.address)
                .FirstOrDefaultAsync(m => m.ordersId == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orders = await _context.orders.FindAsync(id);
            if (orders != null)
            {
                _context.orders.Remove(orders);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ordersExists(int id)
        {
            return _context.orders.Any(e => e.ordersId == id);
        }






        // Add this method to your orders controller (near the bottom)
        private async Task GrantTierOffersForOrder(string userId, string newTier)
        {
            var loyaltyAccount = await _context.loyaltyAccount
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (loyaltyAccount == null) return;

            var currentActiveOffers = string.IsNullOrEmpty(loyaltyAccount.ActiveOffers)
                ? new List<string>()
                : loyaltyAccount.ActiveOffers.Split(',').ToList();

            bool changed = false;

            // Grant offers based on tier (only if not already active)
            if (newTier == "Silver" && !currentActiveOffers.Contains("10% off Fruits & Vegetables"))
            {
                currentActiveOffers.Add("10% off Fruits & Vegetables");
                changed = true;
            }
            else if (newTier == "Gold" && !currentActiveOffers.Contains("Free Cheese"))
            {
                currentActiveOffers.Add("Free Cheese");
                changed = true;
            }
            else if (newTier == "Platinum" && !currentActiveOffers.Contains("£5 Voucher"))
            {
                currentActiveOffers.Add("£5 Voucher");
                changed = true;
            }

            if (changed)
            {
                loyaltyAccount.ActiveOffers = string.Join(",", currentActiveOffers);
                _context.Update(loyaltyAccount);
                await _context.SaveChangesAsync();
            }
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
