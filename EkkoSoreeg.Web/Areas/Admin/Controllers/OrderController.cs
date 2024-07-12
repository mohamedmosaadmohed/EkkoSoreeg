using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Entities.ViewModels;
using EkkoSoreeg.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EkkoSoreeg.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetData()
        {
            IEnumerable<OrderHeader> OrderHeaders;
            OrderHeaders = _unitOfWork.OrderHeader.GetAll(IncludeWord: "applicationUser");
            return Json(new { data = OrderHeaders });
        }
        public IActionResult Details(int orderid)
        {
            OrderVM orderVM = new OrderVM()
            {
                orderHeader = _unitOfWork.OrderHeader.GetFirstorDefault(X => X.Id == orderid, IncludeWord: "applicationUser"),
                orderDetails = _unitOfWork.OrderDetails.GetAll(X => X.OrderHeaderId == orderid, IncludeWord: "product"),
            };

            return View(orderVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails()
        {
            var orderFromDB = _unitOfWork.OrderHeader.GetFirstorDefault(X => X.Id == OrderVM.orderHeader.Id);
            orderFromDB.FirstName = OrderVM.orderHeader.FirstName;
            orderFromDB.LastName = OrderVM.orderHeader.LastName;
            orderFromDB.Address = OrderVM.orderHeader.Address;
            orderFromDB.City = OrderVM.orderHeader.City;
            orderFromDB.Region = OrderVM.orderHeader.Region;
            orderFromDB.PhoneNumber = OrderVM.orderHeader.PhoneNumber;
            orderFromDB.AdditionalPhoneNumber = OrderVM.orderHeader.AdditionalPhoneNumber;
            orderFromDB.AdditionalInformation = OrderVM.orderHeader.AdditionalInformation;
            if (OrderVM.orderHeader.Carrier != null)
            {
                orderFromDB.Carrier = OrderVM.orderHeader.Carrier;
            }
            if (OrderVM.orderHeader.Carrier != null)
            {
                orderFromDB.TrackingNumber = OrderVM.orderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderFromDB);
            _unitOfWork.Complete();
            TempData["Update"] = "Order Has been Updated Successfully";
            return RedirectToAction("Details", "Order", new { orderid = orderFromDB.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveOrder()
        {
            _unitOfWork.OrderHeader.UpdateOrderStatus(OrderVM.orderHeader.Id, SD.Approve, null);
            _unitOfWork.Complete();
            TempData["Update"] = "Order Has been Approved Successfully";
            return RedirectToAction("Details", "Order", new { orderid = OrderVM.orderHeader.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartShipping()
        {
            var orderFromDB = _unitOfWork.OrderHeader.GetFirstorDefault(X => X.Id == OrderVM.orderHeader.Id);

            orderFromDB.TrackingNumber = OrderVM.orderHeader.TrackingNumber;
            orderFromDB.Carrier = OrderVM.orderHeader.Carrier;
            orderFromDB.orderStatus = SD.Shipped;
            orderFromDB.shippingDate = DateTime.Now;

            _unitOfWork.OrderHeader.Update(orderFromDB);
            _unitOfWork.Complete();
            TempData["Update"] = "Order Has been Start Shipping Successfully";
            return RedirectToAction("Details", "Order", new { orderid = OrderVM.orderHeader.Id });
        }
    }
}
