using Microsoft.AspNetCore.Mvc;

namespace EkkoSoreeg.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
