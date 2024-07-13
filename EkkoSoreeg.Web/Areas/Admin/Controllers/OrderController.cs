using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Entities.ViewModels;
using EkkoSoreeg.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using OfficeOpenXml;
using System.Drawing;
using System.Reflection.PortableExecutable;

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

        public IActionResult DownloadExcelSheet()
        {
            IEnumerable<OrderHeader> OrderHeaders;
            OrderHeaders = _unitOfWork.OrderHeader.GetAll(IncludeWord: "applicationUser");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Orders");
                var row = 1;
                // Add header row
                worksheet.Cells[row, 1].Value = "Serial";
                worksheet.Cells[row, 2].Value = "Receiver Name";
                worksheet.Cells[row, 3].Value = "Receiver Phone";
                worksheet.Cells[row, 4].Value = "Receiver Notes";
                worksheet.Cells[row, 5].Value = "Address";
                worksheet.Cells[row, 6].Value = "Product_Content";
                worksheet.Cells[row, 7].Value = "Order_Quantity";
                worksheet.Cells[row, 8].Value = "Total_Price";
                row++;

                // Add order data
                int serialNumber = 1;
                foreach (var header in OrderHeaders)
                {
                    if (header.orderStatus == SD.Shipped && header.Downloader == false)
                    {
                        var orderDetails = _unitOfWork.OrderDetails.GetAll(
                            x => x.OrderHeaderId == header.Id, IncludeWord: "product");
                        worksheet.Cells[row, 1].Value = serialNumber;
                        worksheet.Cells[row, 2].Value = $"{header.applicationUser.FirstName}, {header.applicationUser.LastName}";
                        worksheet.Cells[row, 3].Value = header.PhoneNumber;
                        worksheet.Cells[row, 4].Value = header.AdditionalInformation;
                        worksheet.Cells[row, 5].Value = $"{header.City}, {header.Region}, {header.Address}";
                        worksheet.Cells[row, 8].Value = header.totalPrice;

                        var productContent = "";
                        var orderQuantity  = "";

                        foreach (var details in orderDetails)
                        {
                            productContent += $"{details.product.Name}, {details.product.Price:C}\n";
                            orderQuantity += $"{details.Count:N0}\n";
                        }
                        worksheet.Cells[row, 6].Value = productContent.TrimEnd('\r', '\n');
                        worksheet.Cells[row, 7].Value = orderQuantity.TrimEnd('\r', '\n');
                        header.Downloader = true;
                        _unitOfWork.Complete();
                        row++;
                        serialNumber++;
                    }
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                var fileContents = package.GetAsByteArray();
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
            }
        }
    }
}