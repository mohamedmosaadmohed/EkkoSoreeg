using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Utilities.OTP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace EkkoSoreeg.Web.Areas.Identity.Pages.Account
{
    public class ConfirmOTPForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOtpService _otpService;
        private readonly IEmailSender _emailSender;

        public ConfirmOTPForgotPasswordModel(
            UserManager<ApplicationUser> userManager,
            IOtpService otpService,IEmailSender emailSender)
        {
            _userManager = userManager;
            _otpService = otpService;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Email { get; set; }
            public string OTP { get; set; }
        }

        public IActionResult OnGet(string email)
        {
            Input = new InputModel
            {
                Email = email
            };
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

            // Generate password reset token
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            // Redirect to password reset page with the token and email
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code, email = Input.Email },
                protocol: Request.Scheme);

            return Redirect(callbackUrl);
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
