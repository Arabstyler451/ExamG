using GreenfieldLocalHubWebApp.Data;
using GreenfieldLocalHubWebApp.Models;
using GreenfieldLocalHubWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace GreenfieldLocalHubWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            var producers = await _context.producers.OrderBy(p => p.producerName).ToListAsync();

            ViewBag.Producers = producers;

            // Fetch featured products (include producer) and pass to the view
            var featuredProducts = await _context.products
                .Include(p => p.producers)      // eager load producer
                .OrderByDescending(p => p.productsId) // or whatever ordering you prefer
                .Take(8)                       // fetch a reasonable number for the view
                .ToListAsync();

            return View(featuredProducts);
        }

        public async Task<IActionResult> About()
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            return View();
        }

        public async Task<IActionResult> Contact()
        {
            ViewBag.CartItemCount = await GetCartItemCount();
            ViewBag.Producers = await _context.producers
                .OrderBy(p => p.producerName)
                .ToListAsync();
            return View();
        }


        // POST: /Home/Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(contactEnquiryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Producers = await _context.producers
                    .OrderBy(p => p.producerName)
                    .ToListAsync();
                return View(model);
            }

            // If a producer was selected, attempt to send the email
            if (model.ProducerId.HasValue)
            {
                var producer = await _context.producers
                    .FirstOrDefaultAsync(p => p.producersId == model.ProducerId.Value);

                if (producer != null && !string.IsNullOrWhiteSpace(producer.producerEmail))
                {
                    try
                    {
                        var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587)
                        {
                            Credentials = new System.Net.NetworkCredential(
                                "your-email@gmail.com",   // replace with your sending address
                                "your-app-password"),      // replace with your app password
                            EnableSsl = true
                        };

                        var mail = new System.Net.Mail.MailMessage
                        {
                            From = new System.Net.Mail.MailAddress("your-email@gmail.com",
                                          "Greenfield Local Hub"),
                            Subject = $"[GLH Enquiry] {model.Subject}",
                            Body = $"""
                               You have received a new enquiry via the Greenfield Local Hub contact form.

                               From:    {model.FirstName} {model.LastName}
                               Email:   {model.Email}
                               Subject: {model.Subject}

                               Message:
                               {model.Message}

                               ---
                               Please reply directly to {model.Email}
                               """,
                            IsBodyHtml = false
                        };

                        mail.To.Add(producer.producerEmail);
                        mail.ReplyToList.Add(model.Email);

                        await smtp.SendMailAsync(mail);
                    }
                    catch (Exception ex)
                    {
                        // Log silently — user still sees success
                        Console.WriteLine($"Contact email error: {ex.Message}");
                    }
                }
            }
            // If no producer selected, do nothing — form appears to succeed

            TempData["ContactSuccess"] = "true";
            return RedirectToAction(nameof(Contact));
        }

        public async Task<IActionResult> Privacy()
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            return View();
        }

        public async Task<IActionResult> Terms()
        {
            ViewBag.CartItemCount = await GetCartItemCount();

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
