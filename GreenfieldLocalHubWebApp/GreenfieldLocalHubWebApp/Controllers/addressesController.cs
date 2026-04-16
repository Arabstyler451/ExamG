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
            return View(await _context.address.ToListAsync());
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
            ViewBag.CartItemCount = await GetCartItemCount();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }
            address.UserId = userId;
            ModelState.Remove("UserId");

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
        public async Task<IActionResult> Edit(int id, [Bind("addressId,UserId,street,city,postalCode,country")] address address)
        {
            ViewBag.CartItemCount = await GetCartItemCount();

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

            var address = await _context.address.FindAsync(id);
            if (address != null)
            {
                _context.address.Remove(address);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool addressExists(int id)
        {
            return _context.address.Any(e => e.addressId == id);
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
