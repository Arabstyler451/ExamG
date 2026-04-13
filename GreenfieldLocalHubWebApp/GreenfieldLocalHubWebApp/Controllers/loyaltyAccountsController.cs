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
    public class loyaltyAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public loyaltyAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: loyaltyAccounts
        public async Task<IActionResult> Index()
        {
            return View(await _context.loyaltyAccount.ToListAsync());
        }

        // GET: loyaltyAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyaltyAccount = await _context.loyaltyAccount
                .FirstOrDefaultAsync(m => m.loyaltyAccountId == id);
            if (loyaltyAccount == null)
            {
                return NotFound();
            }

            return View(loyaltyAccount);
        }

        // GET: loyaltyAccounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: loyaltyAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("loyaltyAccountId,UserId,pointsBalance,loyaltyTier")] loyaltyAccount loyaltyAccount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loyaltyAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loyaltyAccount);
        }

        // GET: loyaltyAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyaltyAccount = await _context.loyaltyAccount.FindAsync(id);
            if (loyaltyAccount == null)
            {
                return NotFound();
            }
            return View(loyaltyAccount);
        }

        // POST: loyaltyAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("loyaltyAccountId,UserId,pointsBalance,loyaltyTier")] loyaltyAccount loyaltyAccount)
        {
            if (id != loyaltyAccount.loyaltyAccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loyaltyAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!loyaltyAccountExists(loyaltyAccount.loyaltyAccountId))
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
            return View(loyaltyAccount);
        }

        // GET: loyaltyAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyaltyAccount = await _context.loyaltyAccount
                .FirstOrDefaultAsync(m => m.loyaltyAccountId == id);
            if (loyaltyAccount == null)
            {
                return NotFound();
            }

            return View(loyaltyAccount);
        }

        // POST: loyaltyAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loyaltyAccount = await _context.loyaltyAccount.FindAsync(id);
            if (loyaltyAccount != null)
            {
                _context.loyaltyAccount.Remove(loyaltyAccount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool loyaltyAccountExists(int id)
        {
            return _context.loyaltyAccount.Any(e => e.loyaltyAccountId == id);
        }
    }
}
