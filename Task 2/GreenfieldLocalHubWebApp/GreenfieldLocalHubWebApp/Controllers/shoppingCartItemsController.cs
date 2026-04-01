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
    public class shoppingCartItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public shoppingCartItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: shoppingCartItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.shoppingCartItems.Include(s => s.products).Include(s => s.shoppingCart);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: shoppingCartItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCartItems = await _context.shoppingCartItems
                .Include(s => s.products)
                .Include(s => s.shoppingCart)
                .FirstOrDefaultAsync(m => m.shoppingCartItemsId == id);
            if (shoppingCartItems == null)
            {
                return NotFound();
            }

            return View(shoppingCartItems);
        }

        // GET: shoppingCartItems/Create
        public IActionResult Create()
        {
            ViewData["productsId"] = new SelectList(_context.products, "productsId", "productsId");
            ViewData["shoppingCartId"] = new SelectList(_context.shoppingCart, "shoppingCartId", "shoppingCartId");
            return View();
        }

        // POST: shoppingCartItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("shoppingCartItemsId,shoppingCartId,productsId,unitPrice,quantity")] shoppingCartItems shoppingCartItems)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shoppingCartItems);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["productsId"] = new SelectList(_context.products, "productsId", "productsId", shoppingCartItems.productsId);
            ViewData["shoppingCartId"] = new SelectList(_context.shoppingCart, "shoppingCartId", "shoppingCartId", shoppingCartItems.shoppingCartId);
            return View(shoppingCartItems);
        }

        // GET: shoppingCartItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCartItems = await _context.shoppingCartItems.FindAsync(id);
            if (shoppingCartItems == null)
            {
                return NotFound();
            }
            ViewData["productsId"] = new SelectList(_context.products, "productsId", "productsId", shoppingCartItems.productsId);
            ViewData["shoppingCartId"] = new SelectList(_context.shoppingCart, "shoppingCartId", "shoppingCartId", shoppingCartItems.shoppingCartId);
            return View(shoppingCartItems);
        }

        // POST: shoppingCartItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("shoppingCartItemsId,shoppingCartId,productsId,unitPrice,quantity")] shoppingCartItems shoppingCartItems)
        {
            if (id != shoppingCartItems.shoppingCartItemsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoppingCartItems);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!shoppingCartItemsExists(shoppingCartItems.shoppingCartItemsId))
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
            ViewData["productsId"] = new SelectList(_context.products, "productsId", "productsId", shoppingCartItems.productsId);
            ViewData["shoppingCartId"] = new SelectList(_context.shoppingCart, "shoppingCartId", "shoppingCartId", shoppingCartItems.shoppingCartId);
            return View(shoppingCartItems);
        }

        // GET: shoppingCartItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingCartItems = await _context.shoppingCartItems
                .Include(s => s.products)
                .Include(s => s.shoppingCart)
                .FirstOrDefaultAsync(m => m.shoppingCartItemsId == id);
            if (shoppingCartItems == null)
            {
                return NotFound();
            }

            return View(shoppingCartItems);
        }

        // POST: shoppingCartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoppingCartItems = await _context.shoppingCartItems.FindAsync(id);
            if (shoppingCartItems != null)
            {
                _context.shoppingCartItems.Remove(shoppingCartItems);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool shoppingCartItemsExists(int id)
        {
            return _context.shoppingCartItems.Any(e => e.shoppingCartItemsId == id);
        }
    }
}
