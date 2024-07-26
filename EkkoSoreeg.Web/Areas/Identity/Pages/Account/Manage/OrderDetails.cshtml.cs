using System.Collections.Generic;
using System.Linq;
using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EkkoSoreeg.Web.Pages.Orders
{
    public class OrderDetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderDetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }

        public void OnGet(int id)
        {
            OrderHeader = _unitOfWork.OrderHeader.GetFirstorDefault(o => o.Id == id);
            OrderDetails = _unitOfWork.OrderDetails.GetAll(d => d.OrderHeaderId == id, IncludeWord: "product,product.ProductImages");
        }
    }
}
