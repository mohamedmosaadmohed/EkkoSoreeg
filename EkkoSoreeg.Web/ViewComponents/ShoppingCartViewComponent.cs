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
                var cartData = HttpContext.Request.Cookies[SD.CartKey];
                int cartCount = 0;

                if (!string.IsNullOrEmpty(cartData))
                {
				   var shoppingCartList =	JsonConvert.DeserializeObject<List<ShoppingCart>>(cartData);
					cartCount = shoppingCartList.Count;
                }

                return View(cartCount);
            }
        }

	}
}
