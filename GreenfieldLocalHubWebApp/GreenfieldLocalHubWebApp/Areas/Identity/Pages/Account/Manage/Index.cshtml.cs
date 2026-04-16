// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using GreenfieldLocalHubWebApp.Data;

namespace GreenfieldLocalHubWebApp.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;


        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IEmailSender emailSender,
            ILogger<IndexModel> logger, ApplicationDbContext context)

        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _context = context;

        }

        // =======================
        // Profile Information
        // =======================
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public ProfileInputModel ProfileInput { get; set; }

        public class ProfileInputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        // =======================
        // Email Management
        // =======================
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }

        [BindProperty]
        public EmailInputModel EmailInput { get; set; }

        public class EmailInputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        // =======================
        // Password Management
        // =======================
        public bool HasPassword { get; set; }

        [BindProperty]
        public ChangePasswordInputModel ChangePasswordInput { get; set; }

        public class ChangePasswordInputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            [Required]
            [StringLength(100,
                ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
                MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword",
                ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        // Load User Data
        private async Task LoadAsync(IdentityUser user)
        {
            ViewData["CartItemCount"] = await GetCartItemCount();

            Username = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            Email = await _userManager.GetEmailAsync(user);
            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            HasPassword = await _userManager.HasPasswordAsync(user);

            ProfileInput = new ProfileInputModel
            {
                PhoneNumber = phoneNumber
            };

            EmailInput = new EmailInputModel
            {
                NewEmail = Email
            };
        }




        // GET: Profile Page
        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["CartItemCount"] = await GetCartItemCount();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(
                    $"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            ViewData["ActivePage"] = "Index";

            return Page();
        }



        // POST: Update Profile
        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            ViewData["CartItemCount"] = await GetCartItemCount();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(
                    $"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (ProfileInput.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(
                    user, ProfileInput.PhoneNumber);

                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }



        // POST: Change Email
        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            ViewData["CartItemCount"] = await GetCartItemCount();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(
                    $"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (EmailInput.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(
                    user, EmailInput.NewEmail);

                code = WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new
                    {
                        area = "Identity",
                        userId = userId,
                        email = EmailInput.NewEmail,
                        code = code
                    },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    EmailInput.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by " +
                    $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>" +
                    $"clicking here</a>.");

                StatusMessage =
                    "Confirmation link to change email sent. Please check your email.";
                return RedirectToPage();
            }

            StatusMessage = "Your email is unchanged.";
            return RedirectToPage();
        }



        // POST: Send Verification Email
        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            ViewData["CartItemCount"] = await GetCartItemCount();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(
                    $"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(
                user);

            code = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new
                {
                    area = "Identity",
                    userId = userId,
                    code = code
                },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by " +
                $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>" +
                $"clicking here</a>.");

            StatusMessage =
                "Verification email sent. Please check your email.";
            return RedirectToPage();
        }




        // POST: Change Password
        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            ViewData["CartItemCount"] = await GetCartItemCount();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(
                    $"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!HasPassword)
            {
                return RedirectToPage("./SetPassword");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                ChangePasswordInput.OldPassword,
                ChangePasswordInput.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await LoadAsync(user);
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";
            return RedirectToPage();
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