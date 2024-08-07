using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Utilities.OTP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace EkkoSoreeg.Web.Areas.Identity.Pages.Account
{
    public class ConfirmOTPModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<ConfirmOTPModel> _logger;
        private readonly IOtpService _otpService;

        public ConfirmOTPModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<ConfirmOTPModel> logger,
            IOtpService otpService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _otpService = otpService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Email { get; set; }
            public string OTP { get; set; }
        }

        public IActionResult OnGet(string email, string returnUrl = null)
        {
            Input = new InputModel
            {
                Email = email
            };
            ViewData["ReturnUrl"] = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Verify the OTP
            var isValid = _otpService.VerifyOTP(Input.Email, Input.OTP);
            if (!isValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid OTP.");
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return Page();
            }

            // Confirm the user's email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                _logger.LogInformation("User confirmed their email.");

                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostResendOTPAsync([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // Generate a new OTP
            var otp = GenerateOTP();
            // Store the OTP with an expiration time
            _otpService.StoreOTP(user.Email, otp, TimeSpan.FromMinutes(5));

            // Send the OTP via email
            await _emailSender.SendEmailAsync(user.Email, "OTP Verification", $"<h1>{otp}</h1>");

            return new JsonResult(new { success = true });
        }

        // Method to generate OTP
        private string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(1000, 9999).ToString();
        }
    }
}
