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
    public class orderProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public orderProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: orderProducts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.orderProducts.Include(o => o.orders).Include(o => o.products);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: orderProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderProducts = await _context.orderProducts
                .Include(o => o.orders)
                .Include(o => o.products)
                .FirstOrDefaultAsync(m => m.orderProductsId == id);
            if (orderProducts == null)
            {
                return NotFound();
            }

            return View(orderProducts);
        }

        // GET: orderProducts/Create
        public IActionResult Create()
        {
            ViewData["ordersId"] = new SelectList(_context.Set<orders>(), "ordersId", "ordersId");
            ViewData["productsId"] = new SelectList(_context.Set<products>(), "productsId", "productsId");
            return View();
        }

        // POST: orderProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("orderProductsId,ordersId,productsId,quantity,unitPrice")] orderProducts orderProducts)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderProducts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ordersId"] = new SelectList(_context.Set<orders>(), "ordersId", "ordersId", orderProducts.ordersId);
            ViewData["productsId"] = new SelectList(_context.Set<products>(), "productsId", "productsId", orderProducts.productsId);
            return View(orderProducts);
        }

        // GET: orderProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderProducts = await _context.orderProducts.FindAsync(id);
            if (orderProducts == null)
            {
                return NotFound();
            }
            ViewData["ordersId"] = new SelectList(_context.Set<orders>(), "ordersId", "ordersId", orderProducts.ordersId);
            ViewData["productsId"] = new SelectList(_context.Set<products>(), "productsId", "productsId", orderProducts.productsId);
            return View(orderProducts);
        }

        // POST: orderProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("orderProductsId,ordersId,productsId,quantity,unitPrice")] orderProducts orderProducts)
        {
            if (id != orderProducts.orderProductsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderProducts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!orderProductsExists(orderProducts.orderProductsId))
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
            ViewData["ordersId"] = new SelectList(_context.Set<orders>(), "ordersId", "ordersId", orderProducts.ordersId);
            ViewData["productsId"] = new SelectList(_context.Set<products>(), "productsId", "productsId", orderProducts.productsId);
            return View(orderProducts);
        }

        // GET: orderProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderProducts = await _context.orderProducts
                .Include(o => o.orders)
                .Include(o => o.products)
                .FirstOrDefaultAsync(m => m.orderProductsId == id);
            if (orderProducts == null)
            {
                return NotFound();
            }

            return View(orderProducts);
        }

        // POST: orderProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderProducts = await _context.orderProducts.FindAsync(id);
            if (orderProducts != null)
            {
                _context.orderProducts.Remove(orderProducts);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool orderProductsExists(int id)
        {
            return _context.orderProducts.Any(e => e.orderProductsId == id);
        }
    }
}
