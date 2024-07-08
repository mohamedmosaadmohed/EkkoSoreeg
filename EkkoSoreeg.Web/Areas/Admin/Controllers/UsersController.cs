using EkkoSoreeg.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EkkoSoreeg.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.AdminRole)]
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
