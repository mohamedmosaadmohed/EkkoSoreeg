using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Utilities;
using Microsoft.AspNetCore.Mvc;
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
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            

			if (claim != null)
            {
                if(HttpContext.Session.GetInt32(SD.SessionKey) != null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionKey));
                }
                else
                {
					HttpContext.Session.SetInt32(SD.SessionKey,
                         _unitOfWork.ShoppingCart.GetAll(X => X.ApplicationUserId == claim.Value).ToList().Count());
					return View(HttpContext.Session.GetInt32(SD.SessionKey));
				}
            }
            else
            {
				HttpContext.Session.Clear();
                return View(0);
			}
		}
    }
}
