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
    public class productsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public productsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.products.Include(p => p.categories).Include(p => p.producers);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.products
                .Include(p => p.categories)
                .Include(p => p.producers)
                .FirstOrDefaultAsync(m => m.productsId == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: products/Create
        public IActionResult Create()
        {
            ViewData["categoriesId"] = new SelectList(_context.categories, "categoriesId", "categoriesId");
            ViewData["producersId"] = new SelectList(_context.producers, "producersId", "producersId");
            return View();
        }

        // POST: products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("productsId,producersId,categoriesId,productName,productDescription,stockQuantity,productPrice,productAvailability,productImage")] products products)
        {
            if (ModelState.IsValid)
            {
                _context.Add(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["categoriesId"] = new SelectList(_context.categories, "categoriesId", "categoriesId", products.categoriesId);
            ViewData["producersId"] = new SelectList(_context.producers, "producersId", "producersId", products.producersId);
            return View(products);
        }

        // GET: products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            ViewData["categoriesId"] = new SelectList(_context.categories, "categoriesId", "categoriesId", products.categoriesId);
            ViewData["producersId"] = new SelectList(_context.producers, "producersId", "producersId", products.producersId);
            return View(products);
        }

        // POST: products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("productsId,producersId,categoriesId,productName,productDescription,stockQuantity,productPrice,productAvailability,productImage")] products products)
        {
            if (id != products.productsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!productsExists(products.productsId))
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
            ViewData["categoriesId"] = new SelectList(_context.categories, "categoriesId", "categoriesId", products.categoriesId);
            ViewData["producersId"] = new SelectList(_context.producers, "producersId", "producersId", products.producersId);
            return View(products);
        }

        // GET: products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.products
                .Include(p => p.categories)
                .Include(p => p.producers)
                .FirstOrDefaultAsync(m => m.productsId == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var products = await _context.products.FindAsync(id);
            if (products != null)
            {
                _context.products.Remove(products);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool productsExists(int id)
        {
            return _context.products.Any(e => e.productsId == id);
        }
    }
}
