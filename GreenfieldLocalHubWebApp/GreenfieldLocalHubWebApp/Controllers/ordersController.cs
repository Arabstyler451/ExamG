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
                    .ThenInclude(p => p.producers)
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
            if (userId == null)
            {
                return Unauthorized();
            }

            ViewBag.shoppingCartId = shoppingCartId;

            // Get user's addresses
            var userAddresses = await _context.address
                .Where(a => a.UserId == userId)
                .ToListAsync();

            ViewBag.HasAddresses = userAddresses.Any();
            ViewData["AddressId"] = new SelectList(userAddresses, "addressId", "street", selectedAddressId);

            // Calculate totals for display
            var (subtotalBeforeDiscount, loyaltyDiscount, cartTotalAfterDiscount) =
                await CalculateOrderTotals(userId, shoppingCartId);

            ViewBag.CartTotal = cartTotalAfterDiscount;           // Shown as "Subtotal" in checkout
            ViewBag.SubtotalBeforeDiscount = subtotalBeforeDiscount;
            ViewBag.LoyaltyDiscount = loyaltyDiscount;

            return View();
        }



        // POST: orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ordersId,addressId,deliveryType,orderCollectionDate")] orders orders, int shoppingCartId, string fulfilmentChoice)
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                ViewBag.shoppingCartId = shoppingCartId;
                return View(orders);
            }

            // ── Map FulfilmentChoice to bool fields ──
            orders.delivery = fulfilmentChoice == "Delivery";
            orders.collection = fulfilmentChoice == "Collection";

            // ── Set server-side fields before validation ──
            orders.UserId = userId;
            orders.orderDate = DateOnly.FromDateTime(DateTime.Today);
            orders.orderStatus = "Pending";

            // ── Clear ModelState for every field set in code, not from the form ──
            ModelState.Remove("UserId");
            ModelState.Remove("orderStatus");
            ModelState.Remove("orderDate");
            ModelState.Remove("totalAmount");
            ModelState.Remove("deliveryFee");
            ModelState.Remove("delivery");
            ModelState.Remove("collection");
            ModelState.Remove("DeliveryStreet");
            ModelState.Remove("DeliveryCity");
            ModelState.Remove("DeliveryPostalCode");
            ModelState.Remove("DeliveryCountry");

            // ── Conditional removes based on fulfilment choice ──
            if (orders.collection)
            {
                ModelState.Remove("deliveryType");
                ModelState.Remove("addressId");
                orders.addressId = null;
            }

            if (orders.delivery)
            {
                ModelState.Remove("orderCollectionDate");
                // DON'T remove addressId validation here - keep it
            }

            // ── Business logic validation ──
            if (!orders.collection && !orders.delivery)
                ModelState.AddModelError("delivery", "Please select either delivery or collection.");

            if (orders.collection)
            {
                if (orders.orderCollectionDate == null)
                    ModelState.AddModelError("orderCollectionDate", "Collection date is required.");
                else if (orders.orderCollectionDate.Value < DateOnly.FromDateTime(DateTime.Today.AddDays(2)))
                    ModelState.AddModelError("orderCollectionDate", "Collection date must be at least 2 days from today.");
            }

            if (orders.delivery)
            {
                if (string.IsNullOrWhiteSpace(orders.deliveryType))
                    ModelState.AddModelError("deliveryType", "Please select a delivery type.");

                //Check if address is selected
                if (!orders.addressId.HasValue || orders.addressId.Value <= 0)
                    ModelState.AddModelError("addressId", "Please select a delivery address.");

                //Also verify the address belongs to the user
                if (orders.addressId.HasValue)
                {
                    var addressExists = await _context.address
                        .AnyAsync(a => a.addressId == orders.addressId.Value && a.UserId == userId);

                    if (!addressExists)
                        ModelState.AddModelError("addressId", "Invalid delivery address selected.");
                }
            }

            // ── Load cart ──
            var cart = await _context.shoppingCart
                .FirstOrDefaultAsync(sc => sc.shoppingCartId == shoppingCartId
                                        && sc.UserId == userId
                                        && sc.shoppingCartStatus);

            if (cart == null)
                return NotFound();

            // ── Load cart items ──
            var cartItems = await _context.shoppingCartItems
                .Where(sci => sci.shoppingCartId == shoppingCartId)
                .Include(sci => sci.products)
                    .ThenInclude(p => p.categories)
                .ToListAsync();

            if (!cartItems.Any())
            {
                ModelState.AddModelError("", "Your shopping cart is empty.");
                ViewBag.shoppingCartId = shoppingCartId;
                return View(orders);
            }

            // ── Calculate totals ──
            var (subtotalBeforeDiscount, loyaltyDiscount, cartTotalAfterDiscount) =
                await CalculateOrderTotals(userId, shoppingCartId);

            // ── Delivery fee ──
            decimal deliveryFee = 0m;
            if (orders.delivery)
            {
                deliveryFee = cartTotalAfterDiscount >= 30m ? 0m :
                    orders.deliveryType switch
                    {
                        "First Class" => 5.50m,
                        "Next Day" => 7.99m,
                        _ => 3.50m
                    };
            }

            orders.deliveryFee = (float)deliveryFee;
            orders.totalAmount = (float)(cartTotalAfterDiscount + deliveryFee);

            // ── Snapshot delivery address ──
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

            // ── DEBUG: print any remaining ModelState errors ──
            foreach (var kvp in ModelState)
            {
                foreach (var error in kvp.Value.Errors)
                {
                    Console.WriteLine($"ModelState error — Key: {kvp.Key}, Error: {error.ErrorMessage}");
                }
            }

            if (!ModelState.IsValid)
            {
                // Repopulate ViewBag data for the form
                ViewBag.CartTotal = cartTotalAfterDiscount;
                ViewBag.SubtotalBeforeDiscount = subtotalBeforeDiscount;
                ViewBag.LoyaltyDiscount = loyaltyDiscount;
                ViewBag.shoppingCartId = shoppingCartId;

                // Repopulate addresses dropdown
                var userAddresses = await _context.address
                    .Where(a => a.UserId == userId)
                    .ToListAsync();
                ViewBag.HasAddresses = userAddresses.Any();
                ViewData["AddressId"] = new SelectList(userAddresses, "addressId", "street", orders.addressId);

                return View(orders);
            }

            // ── Save order ──
            _context.orders.Add(orders);
            await _context.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                if (item.products.stockQuantity < item.quantity)
                {
                    ModelState.AddModelError("", $"Sorry, we only have {item.products.stockQuantity} units of {item.products.productName} in stock.");
                    ViewBag.CartTotal = cartTotalAfterDiscount;
                    ViewBag.SubtotalBeforeDiscount = subtotalBeforeDiscount;
                    ViewBag.LoyaltyDiscount = loyaltyDiscount;
                    ViewBag.shoppingCartId = shoppingCartId;
                    return View(orders);
                }

                _context.orderProducts.Add(new orderProducts
                {
                    ordersId = orders.ordersId,
                    productsId = item.productsId,
                    quantity = item.quantity,
                    unitPrice = item.unitPrice
                });

                item.products.stockQuantity -= item.quantity;
            }

            cart.shoppingCartStatus = false;
            await _context.SaveChangesAsync();

            // ── Consume used offers ──
            var usedOffers = new List<string>();
            if (usedOffers.Any())
            {
                var loyaltyAccountForOffers = await _context.loyaltyAccount
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.UserId == userId);

                if (loyaltyAccountForOffers != null)
                {
                    var activeList = string.IsNullOrEmpty(loyaltyAccountForOffers.ActiveOffers)
                        ? new List<string>()
                        : loyaltyAccountForOffers.ActiveOffers.Split(',').Select(s => s.Trim()).ToList();

                    var consumedList = string.IsNullOrEmpty(loyaltyAccountForOffers.ConsumedOffers)
                        ? new List<string>()
                        : loyaltyAccountForOffers.ConsumedOffers.Split(',').Select(s => s.Trim()).ToList();

                    bool changed = false;

                    if (usedOffers.Contains("10% off Fruits & Vegetables"))
                    {
                        if (activeList.Contains("10% off Fruits & Vegetables"))
                        {
                            activeList.Remove("10% off Fruits & Vegetables");
                            if (!consumedList.Contains("10% off Fruits & Vegetables"))
                                consumedList.Add("10% off Fruits & Vegetables");
                            changed = true;
                        }
                    }

                    foreach (var offer in usedOffers)
                    {
                        if (offer == "10% off Fruits & Vegetables") continue;
                        if (activeList.Contains(offer))
                        {
                            activeList.Remove(offer);
                            if (!consumedList.Contains(offer))
                                consumedList.Add(offer);
                            changed = true;
                        }
                    }

                    if (changed)
                    {
                        var accountToUpdate = await _context.loyaltyAccount
                            .FirstOrDefaultAsync(l => l.UserId == userId);

                        if (accountToUpdate != null)
                        {
                            accountToUpdate.ActiveOffers = string.Join(",", activeList.Where(s => !string.IsNullOrWhiteSpace(s)));
                            accountToUpdate.ConsumedOffers = string.Join(",", consumedList.Where(s => !string.IsNullOrWhiteSpace(s)));
                            _context.Update(accountToUpdate);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }

            // ── Award loyalty points ──
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

                var oldTier = loyaltyAccountForPoints.loyaltyTier;

                int pointsEarned = (int)(orders.totalAmount * 10);
                _context.loyaltyTransaction.Add(new loyaltyTransaction
                {
                    loyaltyAccountId = loyaltyAccountForPoints.loyaltyAccountId,
                    ordersId = orders.ordersId,
                    loyaltyPoints = pointsEarned,
                    transactionType = "Earn",
                    transactionDate = DateTime.Now
                });

                loyaltyAccountForPoints.pointsBalance += pointsEarned;
                loyaltyAccountForPoints.loyaltyTier = loyaltyAccountForPoints.pointsBalance switch
                {
                    >= 5000 => "Platinum",
                    >= 2000 => "Gold",
                    >= 500 => "Silver",
                    _ => "Bronze"
                };

                await _context.SaveChangesAsync();

                if (oldTier != loyaltyAccountForPoints.loyaltyTier)
                    await GrantTierOffersForOrder(userId, loyaltyAccountForPoints.loyaltyTier);

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




        private async Task LogOfferConsumption(int loyaltyAccountId, int orderId, IEnumerable<string> consumedOffers)
        {
            foreach (var offer in consumedOffers.Distinct())
            {
                var transaction = new loyaltyTransaction
                {
                    loyaltyAccountId = loyaltyAccountId,
                    ordersId = orderId,
                    loyaltyPoints = 0,
                    transactionType = "Consume",
                    transactionDate = DateTime.Now
                };
                _context.loyaltyTransaction.Add(transaction);
            }
            await _context.SaveChangesAsync();
        }



        private async Task<(decimal subtotalBeforeDiscount, decimal loyaltyDiscount, decimal cartTotalAfterDiscount)> CalculateOrderTotals(string userId, int shoppingCartId)
        {
            var items = await _context.shoppingCartItems
                .Where(sci => sci.shoppingCartId == shoppingCartId)
                .Include(sci => sci.products)
                    .ThenInclude(p => p.categories)
                .ToListAsync();

            if (!items.Any())
                return (0m, 0m, 0m);

            decimal subtotal = items.Sum(i => (decimal)i.products.productPrice * i.quantity);
            decimal discount = 0m;

            var loyaltyAccount = await _context.loyaltyAccount.FirstOrDefaultAsync(l => l.UserId == userId);
            var activeOffers = loyaltyAccount != null && !string.IsNullOrEmpty(loyaltyAccount.ActiveOffers)
                ? loyaltyAccount.ActiveOffers.Split(',').Select(o => o.Trim()).ToList()
                : new List<string>();

            foreach (var item in items)
            {
                decimal price = (decimal)item.products.productPrice * item.quantity;

                if (activeOffers.Contains("10% off Fruits & Vegetables") &&
                    item.products.categories != null &&
                    string.Equals(item.products.categories.categoryName?.Trim(), "Fruit & Veg", StringComparison.OrdinalIgnoreCase))
                {
                    discount += price * 0.10m;
                }

                if (activeOffers.Contains("Free Cheese") &&
                    item.products.productName?.Contains("Cheese", StringComparison.OrdinalIgnoreCase) == true)
                {
                    discount += price;
                }
            }

            if (activeOffers.Contains("£5 Voucher") && subtotal >= 20m)
                discount += 5m;


            return (subtotal, discount, subtotal - discount);
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
