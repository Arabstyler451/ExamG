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
using Microsoft.AspNetCore.Authorization;

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
            
            ViewBag.CartItemCount = await GetCartItemCount();

            // Load categories for the filter dropdown
            ViewBag.Categories = await _context.categories.OrderBy(c => c.categoryName).ToListAsync();


            // Check if the user is in the "producer" role
            if (User.IsInRole("Producer"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if(userId == null)
                {
                    return Unauthorized();
                }

                // Find the producer associated with the current user
                var producer = await _context.producers.FirstOrDefaultAsync(p => p.UserId == userId);

                if (producer == null)
                {
                    return NotFound();
                }

                // Retrieve products associated with the producer
                var producerProducts = await _context.products.Where(p => p.producersId == producer.producersId).Include(p => p.producers).ToListAsync();
                return View(producerProducts);
            }
            else
            {
                // If the user is not a producer, show all products
                var allProducts = await _context.products.Include(p => p.categories).Include(p => p.producers).ToListAsync();
                return View(allProducts);
            }
        }

        // GET: products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.CartItemCount = await GetCartItemCount();

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
        [Authorize(Roles = "Producer, Admin, Developer")]
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
        [Authorize(Roles = "Producer, Admin, Developer")]
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

        [Authorize(Roles = "Producer, Admin, Developer")]
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
            return View(products);
        }

        // POST: products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Producer, Admin, Developer")]
        public async Task<IActionResult> Edit(int id, [Bind("productsId,categoriesId,productName,productDescription,stockQuantity,productPrice,productAvailability,productImage")] products products)
        {

            if (id != products.productsId)
            {
                return NotFound();
            }

            // Get the current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            // Find the producer associated with the current user
            var producer = await _context.producers.FirstOrDefaultAsync(p => p.UserId == userId);
            if (producer == null)
            {
                return NotFound();
            }

            products.producersId = producer.producersId;
            ModelState.Remove("producersId");

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
            return View(products);
        }


        // GET: products/Delete/5
        [Authorize(Roles = "Producer, Admin, Developer")]
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

        // POST: Products/Delete/5
        [Authorize(Roles = "Producer, Admin, Developer")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var products = await _context.products
                .Include(p => p.producers)
                .FirstOrDefaultAsync(p => p.productsId == id);

            if (products == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Only the owner/producer of this product can delete it
            if (products.producers.UserId != userId)
            {
                return Forbid();
            }

            _context.products.Remove(products);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool productsExists(int id)
        {
            return _context.products.Any(e => e.productsId == id);
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


        // GET: /Products/AvailableProducts
        public async Task<IActionResult> AvailableProducts()
        {
            var availableProducts = await _context.products
                .Where(p => p.productAvailability == true)
                .ToListAsync();
            return View("Index", availableProducts);
        }






        public async Task<IActionResult> Search(
            string query,
            int? categoryId,
            string stockFilter,
            string sortBy,
            int page = 1,
            int pageSize = 12)
        {
            // Load categories for dropdowns
            ViewBag.Categories = await _context.categories
                .OrderBy(c => c.categoryName)
                .ToListAsync();

            ViewBag.CurrentQuery = query;
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.CurrentStockFilter = stockFilter;
            ViewBag.CurrentSort = sortBy;
            ViewBag.CurrentPage = page;

            // Start building the query
            IQueryable<products> productsQuery = _context.products
                .AsNoTracking()
                .Include(p => p.categories)
                .Include(p => p.producers);

            // Role-based filtering for Producers
            if (User.IsInRole("Producer"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var producer = await _context.producers
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (producer != null)
                {
                    productsQuery = productsQuery
                        .Where(p => p.producersId == producer.producersId);
                }
            }

            // Apply search query across multiple fields
            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.Trim().ToLower();

                productsQuery = productsQuery.Where(p =>
                    p.productName.ToLower().Contains(query) ||
                    p.productDescription.ToLower().Contains(query) ||
                    p.categories.categoryName.ToLower().Contains(query) ||
                    p.producers.producerName.ToLower().Contains(query));
            }

            // Apply category filter
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                productsQuery = productsQuery
                    .Where(p => p.categoriesId == categoryId.Value);
            }

            // Apply stock filter
            if (!string.IsNullOrEmpty(stockFilter))
            {
                productsQuery = stockFilter switch
                {
                    "in_stock" => productsQuery.Where(p => p.stockQuantity > 0),
                    "out_of_stock" => productsQuery.Where(p => p.stockQuantity == 0),
                    "low_stock" => productsQuery.Where(p => p.stockQuantity <= 10 && p.stockQuantity > 0),
                    _ => productsQuery
                };
            }

            // Apply sorting
            productsQuery = sortBy switch
            {
                "price_asc" => productsQuery.OrderBy(p => p.productPrice),
                "price_desc" => productsQuery.OrderByDescending(p => p.productPrice),
                "name_desc" => productsQuery.OrderByDescending(p => p.productName),
                "name_asc" => productsQuery.OrderBy(p => p.productName),
                _ => productsQuery.OrderBy(p => p.productName)
            };

            // Pagination
            var totalItems = await productsQuery.CountAsync();

            var products = await productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View("Index", products);
        }






        public async Task<IActionResult> FilterProducts(string stockFilter, int? categoryId, string sortBy)
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            // Load categories for the filter dropdown
            ViewBag.Categories = await _context.categories.OrderBy(c => c.categoryName).ToListAsync();

            // Store current values to maintain state in the view
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.CurrentSort = sortBy;
            ViewBag.CurrentStockFilter = stockFilter;

            IQueryable<products> query = _context.products.Include(p => p.categories).Include(p => p.producers);

            // Apply role-based filtering first
            if (User.IsInRole("Producer"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return Unauthorized();
                }

                var producer = await _context.producers.FirstOrDefaultAsync(p => p.UserId == userId);

                if (producer == null)
                {
                    return NotFound();
                }

                query = query.Where(p => p.producersId == producer.producersId);
            }

            // Apply category filter
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.categoriesId == categoryId.Value);
            }

            // Apply stock filter
            if (!string.IsNullOrEmpty(stockFilter))
            {
                switch (stockFilter)
                {
                    case "in_stock":
                        query = query.Where(p => p.stockQuantity > 0);
                        break;
                    case "out_of_stock":
                        query = query.Where(p => p.stockQuantity == 0);
                        break;
                    case "low_stock":
                        query = query.Where(p => p.stockQuantity <= 10 && p.stockQuantity > 0);
                        break;
                }
            }

            // Apply sorting
            switch (sortBy)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.productPrice);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.productPrice);
                    break;
                case "name_asc":
                    query = query.OrderBy(p => p.productName);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(p => p.productName);
                    break;
                default:
                    query = query.OrderBy(p => p.productsId);
                    break;
            }

            var filteredProducts = await query.ToListAsync();
            return View("Index", filteredProducts);
        }
    }
}
