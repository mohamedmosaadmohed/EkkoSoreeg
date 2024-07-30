using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Utilities;
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
				string cartData = Request.Cookies["Cart"];
				int cartCount = 0;

				if (!string.IsNullOrEmpty(cartData))
				{
					var cartList = JsonConvert.DeserializeObject<List<ShoppingCart>>(cartData);
					cartCount = cartList.Count;
				}

				return View(cartCount);
			}
		}

	}
}
