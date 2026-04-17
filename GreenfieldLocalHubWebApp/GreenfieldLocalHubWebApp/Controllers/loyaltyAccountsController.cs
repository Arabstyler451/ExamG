using GreenfieldLocalHubWebApp.Data;
using GreenfieldLocalHubWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GreenfieldLocalHubWebApp.Controllers
{
    [Authorize]
    public class loyaltyAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public loyaltyAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: loyaltyAccounts - This is your Dashboard
        public async Task<IActionResult> Index()
        {
            ViewBag.CartItemCount = await GetCartItemCount();
            ViewData["Layout"] = "_AccountLayout";

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loyaltyAccount = await _context.loyaltyAccount
                .Include(l => l.loyaltyTransaction)
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (loyaltyAccount == null)
            {
                loyaltyAccount = new loyaltyAccount
                {
                    UserId = userId,
                    pointsBalance = 0,
                    loyaltyTier = "Bronze",
                    redeemedOffers = string.Empty,
                    ActiveOffers = string.Empty
                };

                _context.loyaltyAccount.Add(loyaltyAccount);
                await _context.SaveChangesAsync();

                loyaltyAccount = await _context.loyaltyAccount
                    .Include(l => l.loyaltyTransaction)
                    .FirstOrDefaultAsync(l => l.UserId == userId);
            }

            // Redeemed = permanent history (cannot redeem again)
            var redeemedList = string.IsNullOrEmpty(loyaltyAccount.redeemedOffers)
                ? new List<string>()
                : loyaltyAccount.redeemedOffers.Split(',').ToList();

            // Prepare dashboard data
            var pointsMonetaryValue = loyaltyAccount.pointsBalance / 100m;
            var pointsToNextTier = GetPointsToNextTier(loyaltyAccount.pointsBalance, loyaltyAccount.loyaltyTier);
            var nextTierName = GetNextTierName(loyaltyAccount.loyaltyTier);
            var tierProgress = GetTierProgress(loyaltyAccount.pointsBalance, loyaltyAccount.loyaltyTier);

            var totalPointsEarned = loyaltyAccount.loyaltyTransaction?
                .Where(t => t.transactionType == "Earn").Sum(t => t.loyaltyPoints) ?? 0;

            var totalPointsRedeemed = loyaltyAccount.loyaltyTransaction?
                .Where(t => t.transactionType == "Redeem").Sum(t => t.loyaltyPoints) ?? 0;

            var recentTransactions = loyaltyAccount.loyaltyTransaction?
                .OrderByDescending(t => t.transactionDate).Take(10).ToList() ?? new List<loyaltyTransaction>();

            var availableOffers = GetAvailableOffers(loyaltyAccount.pointsBalance, redeemedList);

            ViewBag.PointsMonetaryValue = pointsMonetaryValue;
            ViewBag.PointsToNextTier = pointsToNextTier;
            ViewBag.NextTierName = nextTierName;
            ViewBag.TierProgress = tierProgress;
            ViewBag.TotalPointsEarned = totalPointsEarned;
            ViewBag.TotalPointsRedeemed = totalPointsRedeemed;
            ViewBag.RecentTransactions = recentTransactions;
            ViewBag.AvailableOffers = availableOffers;

            return View(loyaltyAccount);
        }

        // POST: loyaltyAccounts/RedeemOffer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RedeemOffer(string offerTitle, int pointsCost)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loyaltyAccount = await _context.loyaltyAccount.FirstOrDefaultAsync(l => l.UserId == userId);

            if (loyaltyAccount == null || loyaltyAccount.pointsBalance < pointsCost)
            {
                TempData["Error"] = "Insufficient points to redeem this offer.";
                return RedirectToAction(nameof(Index));
            }

            var redeemedList = string.IsNullOrEmpty(loyaltyAccount.redeemedOffers)
                ? new List<string>()
                : loyaltyAccount.redeemedOffers.Split(',').ToList();

            if (redeemedList.Contains(offerTitle))
            {
                TempData["Error"] = $"You have already redeemed the '{offerTitle}' offer.";
                return RedirectToAction(nameof(Index));
            }

            // === Create redemption transaction ===
            var transaction = new loyaltyTransaction
            {
                loyaltyAccountId = loyaltyAccount.loyaltyAccountId,
                loyaltyPoints = pointsCost,
                transactionType = "Redeem",
                transactionDate = DateTime.Now
            };

            loyaltyAccount.pointsBalance -= pointsCost;
            loyaltyAccount.loyaltyTier = GetLoyaltyTier(loyaltyAccount.pointsBalance);

            // Add to permanent history
            redeemedList.Add(offerTitle);
            loyaltyAccount.redeemedOffers = string.Join(",", redeemedList);

            // Add to ACTIVE offers (can be used on next order)
            var activeList = string.IsNullOrEmpty(loyaltyAccount.ActiveOffers)
                ? new List<string>()
                : loyaltyAccount.ActiveOffers.Split(',').ToList();
            activeList.Add(offerTitle);
            loyaltyAccount.ActiveOffers = string.Join(",", activeList);

            _context.loyaltyTransaction.Add(transaction);
            _context.Update(loyaltyAccount);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Successfully redeemed {offerTitle}! {pointsCost} points deducted.";
            return RedirectToAction(nameof(Index));
        }

        // ===================== NEW HELPER =====================
        // Call this AFTER a successful order is placed if a loyalty offer was used
        public async Task ConsumeActiveOfferAsync(string offerTitle)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loyaltyAccount = await _context.loyaltyAccount
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (loyaltyAccount == null) return;

            var activeList = string.IsNullOrEmpty(loyaltyAccount.ActiveOffers)
                ? new List<string>()
                : loyaltyAccount.ActiveOffers.Split(',').ToList();

            if (activeList.Remove(offerTitle))
            {
                loyaltyAccount.ActiveOffers = string.Join(",", activeList);
                _context.Update(loyaltyAccount);
                await _context.SaveChangesAsync();
            }
        }

        // GET: loyaltyAccounts/TransactionHistory
        public async Task<IActionResult> TransactionHistory(int? page, string transactionType = null)
        {
            ViewBag.CartItemCount = await GetCartItemCount();
            ViewData["Layout"] = "_AccountLayout";

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loyaltyAccount = await _context.loyaltyAccount
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (loyaltyAccount == null)
            {
                TempData["Info"] = "No loyalty transactions found. Start shopping to earn points!";
                return RedirectToAction(nameof(Index));
            }

            // Build query
            var query = _context.loyaltyTransaction
                .Where(t => t.loyaltyAccountId == loyaltyAccount.loyaltyAccountId)
                .Include(t => t.orders)
                .AsQueryable();

            // Apply filter
            if (!string.IsNullOrEmpty(transactionType) && transactionType != "All")
            {
                query = query.Where(t => t.transactionType == transactionType);
                ViewBag.CurrentType = transactionType;
            }

            // Pagination
            int pageSize = 15;
            int pageNumber = page ?? 1;
            var transactions = await query
                .OrderByDescending(t => t.transactionDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await query.CountAsync();

            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.TransactionTypes = new List<string> { "All", "Earn", "Redeem" };

            return View(transactions);
        }

        // GET: loyaltyAccounts/GetPointsBalance (API for checkout)
        [HttpGet]
        public async Task<IActionResult> GetPointsBalance()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loyaltyAccount = await _context.loyaltyAccount
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (loyaltyAccount == null)
            {
                return Json(new { points = 0, tier = "Bronze", monetaryValue = 0 });
            }

            return Json(new
            {
                points = loyaltyAccount.pointsBalance,
                tier = loyaltyAccount.loyaltyTier,
                monetaryValue = loyaltyAccount.pointsBalance / 100m
            });
        }

        // POST: loyaltyAccounts/ApplyPoints (for checkout)
        [HttpPost]
        public async Task<IActionResult> ApplyPoints(int pointsToRedeem)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loyaltyAccount = await _context.loyaltyAccount
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (loyaltyAccount != null && loyaltyAccount.pointsBalance >= pointsToRedeem)
            {
                decimal discount = pointsToRedeem / 100m;

                // Store in session for checkout (this part is kept as it's unrelated to offers)
                HttpContext.Session.SetInt32("RedeemedPoints", pointsToRedeem);
                HttpContext.Session.SetString("LoyaltyDiscount", discount.ToString());

                return Json(new
                {
                    success = true,
                    discount = discount,
                    pointsLeft = loyaltyAccount.pointsBalance - pointsToRedeem
                });
            }

            return Json(new { success = false, message = "Insufficient points" });
        }

        // === Other actions remain unchanged (Details, Create, Edit, Delete, etc.) ===
        // GET: loyaltyAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.CartItemCount = await GetCartItemCount();
            if (id == null) return NotFound();

            var loyaltyAccount = await _context.loyaltyAccount
                .Include(l => l.loyaltyTransaction)
                .FirstOrDefaultAsync(m => m.loyaltyAccountId == id);

            if (loyaltyAccount == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loyaltyAccount.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(loyaltyAccount);
        }

        // GET: loyaltyAccounts/Create
        public IActionResult Create() => View();

        // POST: loyaltyAccounts/Create
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
            if (id == null) return NotFound();

            var loyaltyAccount = await _context.loyaltyAccount.FindAsync(id);
            if (loyaltyAccount == null) return NotFound();

            if (!User.IsInRole("Admin")) return Forbid();

            return View(loyaltyAccount);
        }

        // POST: loyaltyAccounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("loyaltyAccountId,UserId,pointsBalance,loyaltyTier")] loyaltyAccount loyaltyAccount)
        {
            if (id != loyaltyAccount.loyaltyAccountId) return NotFound();

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
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(loyaltyAccount);
        }

        // GET: loyaltyAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var loyaltyAccount = await _context.loyaltyAccount
                .FirstOrDefaultAsync(m => m.loyaltyAccountId == id);

            if (loyaltyAccount == null) return NotFound();

            if (!User.IsInRole("Admin")) return Forbid();

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

        // Controller method to display amount of items in the shopping cart
        public async Task<int> GetCartItemCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return 0;

            var shoppingCart = await _context.shoppingCart
                .FirstOrDefaultAsync(c => c.UserId == userId && c.shoppingCartStatus);

            if (shoppingCart == null) return 0;

            return await _context.shoppingCartItems
                .Where(sci => sci.shoppingCartId == shoppingCart.shoppingCartId)
                .SumAsync(sci => sci.quantity);
        }

        // ==================== HELPER METHODS ====================
        private string GetLoyaltyTier(int pointsBalance)
        {
            if (pointsBalance >= 5000) return "Platinum";
            if (pointsBalance >= 2000) return "Gold";
            if (pointsBalance >= 500) return "Silver";
            return "Bronze";
        }

        private int GetPointsToNextTier(int currentPoints, string currentTier)
        {
            return currentTier switch
            {
                "Bronze" => 500 - currentPoints,
                "Silver" => 2000 - currentPoints,
                "Gold" => 5000 - currentPoints,
                "Platinum" => 0,
                _ => 0
            };
        }

        private string GetNextTierName(string currentTier)
        {
            return currentTier switch
            {
                "Bronze" => "Silver",
                "Silver" => "Gold",
                "Gold" => "Platinum",
                "Platinum" => "Maximum Tier Reached",
                _ => "Silver"
            };
        }

        private int GetTierProgress(int currentPoints, string currentTier)
        {
            return currentTier switch
            {
                "Bronze" => (currentPoints * 100) / 500,
                "Silver" => ((currentPoints - 500) * 100) / 1500,
                "Gold" => ((currentPoints - 2000) * 100) / 3000,
                "Platinum" => 100,
                _ => 0
            };
        }

        private List<LoyaltyOfferViewModel> GetAvailableOffers(int pointsBalance, List<string> redeemedOffers)
        {
            var allOffers = new List<LoyaltyOfferViewModel>
            {
                new LoyaltyOfferViewModel
                {
                    Title = "10% off Vegetables",
                    Description = "10% off your next vegetable order",
                    PointsCost = 200,
                    IsAvailable = pointsBalance >= 200,
                    IsRedeemed = redeemedOffers.Contains("10% off Vegetables")
                },
                new LoyaltyOfferViewModel
                {
                    Title = "Free Cheese",
                    Description = "One 200g cheese of your choice",
                    PointsCost = 500,
                    IsAvailable = pointsBalance >= 500,
                    IsRedeemed = redeemedOffers.Contains("Free Cheese")
                },
                new LoyaltyOfferViewModel
                {
                    Title = "Free Delivery",
                    Description = "Free delivery on your next order",
                    PointsCost = 300,
                    IsAvailable = pointsBalance >= 300,
                    IsRedeemed = redeemedOffers.Contains("Free Delivery")
                },
                new LoyaltyOfferViewModel
                {
                    Title = "£5 Voucher",
                    Description = "£5 off any order over £20",
                    PointsCost = 800,
                    IsAvailable = pointsBalance >= 800,
                    IsRedeemed = redeemedOffers.Contains("£5 Voucher")
                }
            };

            return allOffers;
        }
    }

    // ViewModel for offers
    public class LoyaltyOfferViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int PointsCost { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsRedeemed { get; set; }
    }
}