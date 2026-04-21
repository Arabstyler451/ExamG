using GreenfieldLocalHubWebApp.Data;
using GreenfieldLocalHubWebApp.Models;
using Microsoft.AspNetCore.Identity;
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
    public class addressesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public addressesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: addresses
        public async Task<IActionResult> Index()
        {
            ViewBag.CartItemCount = await GetCartItemCount();
            ViewData["Layout"] = "_AccountLayout";

            // Get the currently logged-in user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var addresses = _context.address
                   .Where(a => a.UserId == userId)
                   .OrderByDescending(a => a.IsDefault)  // moves default address to the top
                   .ThenByDescending(a => a.createdDate)  //sort the newest addresses added first
                   .ToList();

            // Check if there's only one address and it's not already set as default
            if (addresses.Count == 1 && !addresses[0].IsDefault)
            {
                addresses[0].IsDefault = true;
                _context.address.Update(addresses[0]);
                await _context.SaveChangesAsync();
            }

            // Ensure there's at least one default address
            if (addresses.Any() && !addresses.Any(a => a.IsDefault))
            {
                // If no address is marked as default, set the most recent as default
                addresses[0].IsDefault = true;
                _context.address.Update(addresses[0]);
                await _context.SaveChangesAsync();
            }

            return View(addresses);
        }

        // GET: addresses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.address
                .FirstOrDefaultAsync(m => m.addressId == id);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // GET: addresses/Create
        public IActionResult Create(string returnUrl = null)
        {
            ViewBag.CartItemCount = GetCartItemCount();
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: addresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("addressId,street,city,postalCode,country")] address address, string returnUrl = null)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }
            address.UserId = userId;
            ModelState.Remove("UserId");

            ViewBag.CartItemCount = await GetCartItemCount();


            if (ModelState.IsValid)
            {
                _context.Add(address);
                await _context.SaveChangesAsync();

                // Return to checkout and pre-select the newly created address
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl + "&selectedAddressId=" + address.addressId);
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(address);
        }

        // GET: addresses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.address.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }
            return View(address);
        }

        // POST: addresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("addressId,street,city,postalCode,country")] address address)
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }
            address.UserId = userId;
            ModelState.Remove("UserId");

            if (id != address.addressId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(address);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!addressExists(address.addressId))
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
            return View(address);
        }

        // GET: addresses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.address
                .FirstOrDefaultAsync(m => m.addressId == id);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }



        // POST: addresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            // Find the address with its related orders
            var address = await _context.address
                .Include(a => a.orders)
                .FirstOrDefaultAsync(a => a.addressId == id && a.UserId == userId);

            if (address == null)
            {
                return NotFound();
            }

            // Check if this address is used in any orders
            var linkedOrders = address.orders?.Where(o => o.addressId == id).ToList();

            if (linkedOrders != null && linkedOrders.Any())
            {
                // Break the foreign key relationship by setting addressId to null on all linked orders
                foreach (var order in linkedOrders)
                {
                    order.addressId = null;
                }

                // Now it's safe to delete the address
                _context.address.Remove(address);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Address deleted successfully. It was used in {linkedOrders.Count} order(s), but those orders still have the address details saved.";
            }
            else
            {
                // No orders linked - safe to delete directly
                _context.address.Remove(address);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Address deleted successfully.";
            }

            // If the deleted address was the default, set another address as default
            if (address.IsDefault)
            {
                var newDefaultAddress = await _context.address
                    .Where(a => a.UserId == userId && a.addressId != id)
                    .OrderByDescending(a => a.createdDate)
                    .FirstOrDefaultAsync();

                if (newDefaultAddress != null)
                {
                    newDefaultAddress.IsDefault = true;
                    await _context.SaveChangesAsync();
                    TempData["Info"] = "Another address has been set as your default.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool addressExists(int id)
        {
            return _context.address.Any(e => e.addressId == id);
        }


        // Controller method to set an address as default for the user
        [HttpPost]
        public IActionResult SetDefault(int id)
        {
            // Get all addresses for the current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var addresses = _context.address.Where(a => a.UserId == userId).ToList();

            // Find the address to set as default
            var addressToSetDefault = addresses.FirstOrDefault(a => a.addressId == id);
            if (addressToSetDefault == null)
            {
                return NotFound();
            }

            // Remove default flag from all addresses
            foreach (var addr in addresses)
            {
                addr.IsDefault = false;
            }

            // Set the selected address as default
            addressToSetDefault.IsDefault = true;

            _context.SaveChanges();

            return RedirectToAction("Index");
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
