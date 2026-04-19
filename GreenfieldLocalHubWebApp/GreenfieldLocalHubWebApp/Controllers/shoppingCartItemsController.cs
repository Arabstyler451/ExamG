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
            ViewBag.CartItemCount = await GetCartItemCount();

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
        public async Task<IActionResult> Create(int productsId, int quantity = 1)
        {
            // Clamp quantity to a valid range
            if (quantity < 1) quantity = 1;

            var product = await _context.products.FirstOrDefaultAsync(p => p.productsId == productsId);

            if (product == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var shoppingCart = await _context.shoppingCart
                .FirstOrDefaultAsync(c => c.UserId == userId && c.shoppingCartStatus == true);

            if (shoppingCart == null)
            {
                shoppingCart = new shoppingCart
                {
                    UserId = userId,
                    shoppingCartCreatedAt = DateTime.Now,
                    shoppingCartStatus = true
                };
                _context.shoppingCart.Add(shoppingCart);
                await _context.SaveChangesAsync();
            }

            var shoppingCartItem = await _context.shoppingCartItems
                .FirstOrDefaultAsync(sc => sc.shoppingCartId == shoppingCart.shoppingCartId && sc.productsId == productsId);

            if (shoppingCartItem != null)
            {
                // Add the chosen quantity on top of whatever is already in the cart,
                // but never exceed available stock
                shoppingCartItem.quantity = Math.Min(
                    shoppingCartItem.quantity + quantity,
                    product.stockQuantity
                );
            }
            else
            {
                shoppingCartItem = new shoppingCartItems
                {
                    shoppingCartId = shoppingCart.shoppingCartId,
                    productsId = productsId,
                    quantity = Math.Min(quantity, product.stockQuantity)
                };
                _context.shoppingCartItems.Add(shoppingCartItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "products");
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
            return RedirectToAction("Index", "shoppingCarts");
        }

        private bool shoppingCartItemsExists(int id)
        {
            return _context.shoppingCartItems.Any(e => e.shoppingCartItemsId == id);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int shoppingCartItemsId, int change)
        {
            var item = await _context.shoppingCartItems
                .Include(i => i.products)
                .FirstOrDefaultAsync(i => i.shoppingCartItemsId == shoppingCartItemsId);

            if (item == null)
                return NotFound();

            var newQty = item.quantity + change;

            if (newQty <= 0)
            {
                // Remove the item entirely if quantity drops to zero
                _context.shoppingCartItems.Remove(item);
            }
            else
            {
                // Cap at available stock
                item.quantity = Math.Min(newQty, item.products?.stockQuantity ?? newQty);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "shoppingCarts");
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
