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
    public class producersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public producersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: producers
        public async Task<IActionResult> Index()
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            return View(await _context.producers.ToListAsync());
        }

        // GET: producers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            if (id == null)
            {
                return NotFound();
            }

            var producers = await _context.producers
                .FirstOrDefaultAsync(m => m.producersId == id);
            if (producers == null)
            {
                return NotFound();
            }

            return View(producers);
        }

        [Authorize(Roles = "Admin, Developer")]
        // GET: producers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: producers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Developer")]
        public async Task<IActionResult> Create([Bind("producersId,UserId,producerName,producerEmail,producerPhone,producerDescription,producerLocation,producerImage")] producers producers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(producers);
        }

        // GET: producers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producers = await _context.producers.FindAsync(id);
            if (producers == null)
            {
                return NotFound();
            }
            return View(producers);
        }

        // POST: producers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("producersId,UserId,producerName,producerEmail,producerPhone,producerDescription,producerLocation,producerImage")] producers producers)
        {
            if (id != producers.producersId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!producersExists(producers.producersId))
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
            return View(producers);
        }

        // GET: producers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producers = await _context.producers
                .FirstOrDefaultAsync(m => m.producersId == id);
            if (producers == null)
            {
                return NotFound();
            }

            return View(producers);
        }

        // POST: producers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producers = await _context.producers.FindAsync(id);
            if (producers != null)
            {
                _context.producers.Remove(producers);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool producersExists(int id)
        {
            return _context.producers.Any(e => e.producersId == id);
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
