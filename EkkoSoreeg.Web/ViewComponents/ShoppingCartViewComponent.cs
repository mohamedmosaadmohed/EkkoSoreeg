using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Utilities;
using EkkoSoreeg.Web.DataSeed;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
namespace EkkoSoreeg.Web.ViewComponents
{
	public class ShoppingCartViewComponent : ViewComponent
	{
		private IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var claimsIdentity = User.Identity as ClaimsIdentity;
			var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

			if (claim != null)
			{
				if (HttpContext.Session.GetInt32(SD.SessionKey) != null)
				{
					return View(HttpContext.Session.GetInt32(SD.SessionKey));
				}
				else
				{
					HttpContext.Session.SetInt32(SD.SessionKey,
						_unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count());
					return View(HttpContext.Session.GetInt32(SD.SessionKey));
				}
			}
			else
			{
                var cartItems = HttpContext.Session.GetObjectFromJson<List<ShoppingCart>>(SD.CartKey);
                int cartCount = cartItems?.Count ?? 0;
                return View(cartCount);
            }
		}

	}
}
