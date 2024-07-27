using EkkoSoreeg.DataAccess.Data;
using EkkoSoreeg.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EkkoSoreeg.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ContactController : Controller
    {
        private readonly IEmailSender _emailSender;
        public ContactController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendContact(Contact contact)
        {
            if (ModelState.IsValid)
            {
                string template = $@"
            <h2>Contact Request</h2>
            <p><strong>Name:</strong> {contact.Name}</p>
            <p><strong>PhoneNumber:</strong> {contact.Phone}</p>
            <p><strong>Email:</strong> {contact.Email}</p>
            <p><strong>Message:</strong> {contact.Message}</p>";

                try
                {
                    await _emailSender.SendEmailAsync("momosaadhenaish555@gmail.com", "Contact Request", template);

                    TempData["Order"] = "Message Has Been Sended Successfuly";
                    return RedirectToAction("Index", "Contact");
                }
                catch (InvalidOperationException)
                {
                    TempData["Delete"] = "Message Has Not Sended Try again!";
					return RedirectToAction("Index", "Contact");
				}
            }

			TempData["Delete"] = "Message Has Not Sended Try again!";
			return RedirectToAction("Index", "Contact");
		}
    }
}
