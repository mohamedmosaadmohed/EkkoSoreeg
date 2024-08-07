using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
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
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IOtpService _otpService;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager,
            IEmailSender emailSender, IOtpService otpService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _otpService  = otpService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    ModelState.AddModelError(string.Empty, "The email address does not exist or is not confirmed.");
                    return Page();
                }

                // Generate OTP
                var otp = GenerateOTP();
                // Store OTP with expiration time
                _otpService.StoreOTP(user.Email, otp, TimeSpan.FromMinutes(5));

                // Send OTP via Email
                await _emailSender.SendEmailAsync(user.Email, "OTP Verification", $"<h1>{otp}</h1>");

                // Redirect to OTP confirmation page
                return RedirectToPage("ConfirmOTPForgotPassword", new { email = Input.Email });
            }

            return Page();
        }

        // Method to generate OTP
        private string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(1000, 9999).ToString();
        }
    }
}

