using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Utilities;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace EkkoSoreeg.Web.DataSeed
{
	public class CartService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IUnitOfWork _unitOfWork;

		public CartService(UserManager<ApplicationUser> userManager,
						   IHttpContextAccessor httpContextAccessor,
						   IUnitOfWork unitOfWork)
		{
			_userManager = userManager;
			_httpContextAccessor = httpContextAccessor;
			_unitOfWork = unitOfWork;
		}

		public async Task TransferCartDataAsync()
		{
			var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
			if (user == null)
				return;

			var cartCookie = _httpContextAccessor.HttpContext.Request.Cookies[SD.CartKey];
			if (!string.IsNullOrEmpty(cartCookie))
			{
				var cartItemsFromCookie = JsonConvert.DeserializeObject<List<ShoppingCart>>(cartCookie);

				foreach (var item in cartItemsFromCookie)
				{
					var existingCartItem = _unitOfWork.ShoppingCart.GetFirstorDefault(
						x => x.ProductId == item.ProductId && x.Color == item.Color && x.Size == item.Size && x.ApplicationUserId == user.Id);

					if (existingCartItem != null)
					{
						existingCartItem.Count += item.Count;
					}
					else
					{
						var newCartItem = new ShoppingCart
						{
							shoppingIdGuid = item.shoppingIdGuid,
							ProductId = item.ProductId,
							Color = item.Color,
							Size = item.Size,
							ApplicationUserId = user.Id,
							Count = item.Count,
						};
						_unitOfWork.ShoppingCart.Add(newCartItem);
					}
				}

				_unitOfWork.Complete();
				_httpContextAccessor.HttpContext.Response.Cookies.Delete(SD.CartKey);

				// Optionally update session count
				var cartItemCount = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == user.Id).Count();
				_httpContextAccessor.HttpContext.Session.SetInt32(SD.SessionKey, cartItemCount);
			}
		}
	}
}
