using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GreenfieldLocalHubWebApp.Data;
using GreenfieldLocalHubWebApp.Models;

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
            return View(await _context.shoppingCart.ToListAsync());
        }

        // GET: shoppingCarts/Details/5
        public async Task<IActionResult> Details(int? id)
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
    }
}
