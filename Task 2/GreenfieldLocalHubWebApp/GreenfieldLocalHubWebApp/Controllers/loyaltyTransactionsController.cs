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
    public class loyaltyTransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public loyaltyTransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: loyaltyTransactions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.loyaltyTransaction.Include(l => l.loyaltyAccount).Include(l => l.orders);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: loyaltyTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyaltyTransaction = await _context.loyaltyTransaction
                .Include(l => l.loyaltyAccount)
                .Include(l => l.orders)
                .FirstOrDefaultAsync(m => m.loyaltyTransactionId == id);
            if (loyaltyTransaction == null)
            {
                return NotFound();
            }

            return View(loyaltyTransaction);
        }

        // GET: loyaltyTransactions/Create
        public IActionResult Create()
        {
            ViewData["loyaltyAccountId"] = new SelectList(_context.loyaltyAccount, "loyaltyAccountId", "loyaltyAccountId");
            ViewData["ordersId"] = new SelectList(_context.Set<orders>(), "ordersId", "ordersId");
            return View();
        }

        // POST: loyaltyTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("loyaltyTransactionId,loyaltyAccountId,ordersId,loyaltyPoints,transactionType,transactionDate")] loyaltyTransaction loyaltyTransaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loyaltyTransaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["loyaltyAccountId"] = new SelectList(_context.loyaltyAccount, "loyaltyAccountId", "loyaltyAccountId", loyaltyTransaction.loyaltyAccountId);
            ViewData["ordersId"] = new SelectList(_context.Set<orders>(), "ordersId", "ordersId", loyaltyTransaction.ordersId);
            return View(loyaltyTransaction);
        }

        // GET: loyaltyTransactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyaltyTransaction = await _context.loyaltyTransaction.FindAsync(id);
            if (loyaltyTransaction == null)
            {
                return NotFound();
            }
            ViewData["loyaltyAccountId"] = new SelectList(_context.loyaltyAccount, "loyaltyAccountId", "loyaltyAccountId", loyaltyTransaction.loyaltyAccountId);
            ViewData["ordersId"] = new SelectList(_context.Set<orders>(), "ordersId", "ordersId", loyaltyTransaction.ordersId);
            return View(loyaltyTransaction);
        }

        // POST: loyaltyTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("loyaltyTransactionId,loyaltyAccountId,ordersId,loyaltyPoints,transactionType,transactionDate")] loyaltyTransaction loyaltyTransaction)
        {
            if (id != loyaltyTransaction.loyaltyTransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loyaltyTransaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!loyaltyTransactionExists(loyaltyTransaction.loyaltyTransactionId))
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
            ViewData["loyaltyAccountId"] = new SelectList(_context.loyaltyAccount, "loyaltyAccountId", "loyaltyAccountId", loyaltyTransaction.loyaltyAccountId);
            ViewData["ordersId"] = new SelectList(_context.Set<orders>(), "ordersId", "ordersId", loyaltyTransaction.ordersId);
            return View(loyaltyTransaction);
        }

        // GET: loyaltyTransactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyaltyTransaction = await _context.loyaltyTransaction
                .Include(l => l.loyaltyAccount)
                .Include(l => l.orders)
                .FirstOrDefaultAsync(m => m.loyaltyTransactionId == id);
            if (loyaltyTransaction == null)
            {
                return NotFound();
            }

            return View(loyaltyTransaction);
        }

        // POST: loyaltyTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loyaltyTransaction = await _context.loyaltyTransaction.FindAsync(id);
            if (loyaltyTransaction != null)
            {
                _context.loyaltyTransaction.Remove(loyaltyTransaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool loyaltyTransactionExists(int id)
        {
            return _context.loyaltyTransaction.Any(e => e.loyaltyTransactionId == id);
        }
    }
}
