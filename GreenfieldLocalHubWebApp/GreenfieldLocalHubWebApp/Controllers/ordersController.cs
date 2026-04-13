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
                return View(allOrders);
            }

            //If the user is a producer, show only orders that contain their products. Otherwise, show only the user's own orders.
            else if (User.IsInRole("Producer"))
            {
                var producerProducts = await _context.products.Where(p => p.producers.UserId == userId).Select(p => p.productsId).ToListAsync();
                var producerOrders = await _context.orderProducts.Where(op => producerProducts.Contains(op.productsId)).Include(op => op.orders).Include(op => op.products).ToListAsync();
                return View(producerOrders.Select(vo => vo.orders).Distinct().ToList());
            }
            else
            {
                var orders = await _context.orders.Where(o => o.UserId == userId).Include(o => o.orderProducts).ThenInclude(op => op.products).ToListAsync();
                return View(orders);
            }

        }


        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderProducts = await _context.orderProducts
                .Where(op => op.ordersId == id)
                .Include(op => op.orders)
                .Include(op => op.products)
                .ToListAsync();

            if (!orderProducts.Any())   // ← Fixed the check
            {
                return NotFound();
            }

            return View(orderProducts);   // ← Pass the list directly
        }

        // GET: orders/Create
        public async Task<IActionResult> Create(int shoppingCartId, int? selectedAddressId = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewBag.shoppingCartId = shoppingCartId;

            // Get user's addresses
            var userAddresses = await _context.address.Where(a => a.UserId == userId).ToListAsync();

            ViewBag.HasAddresses = userAddresses.Any();

            // Build dropdown (pre-select the newly added address if coming back from Addresses/Create)
            ViewData["AddressId"] = new SelectList(userAddresses,"addressId","street",selectedAddressId);
            return View();
        }

        // POST: orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ordersId,addressId,delivery,collection,deliveryType,orderCollectionDate")] orders orders, int shoppingCartId)
        {
            // Set the UserId of the order to the currently logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                ViewBag.shoppingCartId = shoppingCartId;
                return View(orders);
            }

            orders.UserId = userId;
            ModelState.Remove("UserId");


            // Set the order date to the current date
            orders.orderDate = DateOnly.FromDateTime(DateTime.Today);
            ModelState.Remove("orderDate");

            // Set the order status to "Pending"
            orders.orderStatus = "Pending";
            ModelState.Remove("orderStatus");



            var shoppingCart = await _context.shoppingCart.FirstOrDefaultAsync(sc => sc.shoppingCartId == shoppingCartId && sc.UserId == userId && sc.shoppingCartStatus);

            if (shoppingCart == null)
            {
                return NotFound();
            }

            // Retrieve the shopping cart items for the specified cartId
            var shoppingCartItems = await _context.shoppingCartItems.Where(sci => sci.shoppingCartId == shoppingCartId).Include(sci => sci.products).ToListAsync();

            if (!shoppingCartItems.Any())
            {
                ModelState.AddModelError("", "Your shopping cart is empty.");
                ViewBag.shoppingCartId = shoppingCartId;
                return View(orders);
            }


            // Calculate the total amount for the order based on the shopping cart items
            float subTotal = 0.00f;
            foreach (var shoppingCartItem in shoppingCartItems)
            {
                var productTotal = shoppingCartItem.products.productPrice * shoppingCartItem.quantity;
                subTotal = productTotal + subTotal;
            }


            // Check if the user is eligible for a discount based on their order history
            var orderCount = await _context.orders.CountAsync(oc => oc.UserId == userId);

            float discount = 0.00f;

            if (orderCount >= 5)
            {
                discount = subTotal * 0.10f; // 10% discount for customers with 5 or more orders
            }

            orders.totalAmount = subTotal - discount;
            ModelState.Remove("totalAmount");



            // If an address is selected, populate the delivery address fields in the orders model
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



            // Validate that either delivery or collection is selected, but not both
            if (!orders.collection && !orders.delivery)
            {
                ModelState.AddModelError("delivery", "Please select either delivery or collection for your order");
            }

            // If delivery is selected, validate that a delivery type is chosen
            if (orders.collection)
            {
                ModelState.Remove("deliveryType");

                // Validate that a collection date is selected and is at least 2 days from today
                if (orders.orderCollectionDate == null)
                {
                    ModelState.AddModelError("orderCollectionDate", "Collection date is required.");
                }
                else
                {
                    var earliestCollectionDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2));

                    if (orders.orderCollectionDate.Value < earliestCollectionDate)
                    {
                        ModelState.AddModelError("orderCollectionDate", $"Collection date must be at least 2 days from today ({earliestCollectionDate:MM/dd/yyyy}).");
                    }
                }
            }


            if (orders.delivery)
            {
                ModelState.Remove("orderCollectionDate");
                if (string.IsNullOrWhiteSpace(orders.deliveryType))
                {
                    ModelState.AddModelError("deliveryType", "Please select a delivery type for your order");
                }
            }

            // If the model state is valid, save the order to the database
            if (!ModelState.IsValid)
            {
                ViewBag.shoppingCartId = shoppingCartId;
                return View(orders);
            }

            
            _context.orders.Add(orders);
            await _context.SaveChangesAsync();

            // Create orderProducts entries for each shopping cart item and update product stock quantities
            foreach (var shoppingCartItem in shoppingCartItems)
            {
                if (shoppingCartItem.products.stockQuantity < shoppingCartItem.quantity)
                {
                    ModelState.AddModelError("", $"Sorry, we only have {shoppingCartItem.products.stockQuantity} units of {shoppingCartItem.products.productName} in stock. Please adjust the quantity in your shopping cart.");
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

            return RedirectToAction("Index", "Home");
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
    }
}
