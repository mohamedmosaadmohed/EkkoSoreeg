

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Entities.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EkkoSoreeg.Web.Areas.Identity.Pages.Account.Manage
{
    public class OrdersModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrdersModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<OrderHeader> OrderHeaders { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }

        public void OnGet()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderHeaders = _unitOfWork.OrderHeader.GetAll(o => o.ApplicationUserId == claim.Value);

            var orderHeaderIds = OrderHeaders.Select(o => o.Id);
            OrderDetails = _unitOfWork.OrderDetails.GetAll(d => orderHeaderIds.Contains(d.OrderHeaderId), IncludeWord: "product");
        }
    }
}
