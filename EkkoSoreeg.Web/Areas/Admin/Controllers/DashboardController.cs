using EkkoSoreeg.DataAccess.Data;
using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Entities.ViewModels;
using EkkoSoreeg.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EkkoSoreeg.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
	public class DashboardController : Controller
	{
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public DashboardController(ApplicationDbContext context,IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            // Get the current signed-in user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claims.Value;

            var users = _context.TbapplicationUser.Where(x => x.Id != userId).ToList();
            var orders = _unitOfWork.OrderHeader.GetAll();

            var usersCount = users.Count();
            var ordersNumber = orders.Count();
            var cancelledOrders = orders.Where(o => o.orderStatus == SD.Cancelled);
            var totalRevenueCancelled = cancelledOrders.Sum(o => o.totalPrice);

            // Static data for categories
            var productSalesLabels = new List<string> { "T-Shirt", "Jeans", "Shorts", "Pants" };
            var productSalesData = new List<int> { 44, 55, 13, 43 };

            // Static data for monthly sales and revenue
            var monthlySales = new List<int> { 100, 20, 80, 40, 50, 90, 120 };
            var monthlyRevenue = new List<int> { 100, 200, 300, 400, 500, 600, 700 };

            var reportVM = new ReportVM
            {
                UsersCount = usersCount,
                OrdersNumber = ordersNumber,
                TotalRevenue = totalRevenueCancelled,
                MonthlySales = monthlySales,
                MonthlyRevenue = monthlyRevenue,
                ProductSalesLabels = productSalesLabels,
                ProductSalesData = productSalesData
            };

            return View(reportVM);
        }




    }
}
