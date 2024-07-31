using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Entities.ViewModels;
using EkkoSoreeg.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Security.Claims;
using System.Web;

namespace EkkoSoreeg.Web.Areas.Customer.Controllers
{

	[Area("Customer")]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public ShoppingCartVM shoppingCartVM { get; set; }
		public CartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			var claimsIdentity = User.Identity as ClaimsIdentity;
			var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

			var shoppingCartVM = new ShoppingCartVM();

			// Initialize the total cart amounts
			shoppingCartVM.totalCarts = 0;
			shoppingCartVM.totalCartsWithShipping = 0;
			shoppingCartVM.shoppingCarts = new List<ShoppingCart>();

			// Retrieve cart items for logged-in users
			if (claim != null)
			{
				// Retrieve items from the database
				var dbCartItems = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId ==
				     claim.Value, IncludeWord: "Product,Product.ProductImages").ToList();
				shoppingCartVM.shoppingCarts = dbCartItems;

				// Calculate totals
				foreach (var item in dbCartItems)
				{
					shoppingCartVM.totalCarts += (item.Count * item.Product.Price);
				}
			}
			else
			{
				// Retrieve items from cookies for anonymous users
				var cookieCartData = HttpContext.Request.Cookies[SD.CartKey];
				if (!string.IsNullOrEmpty(cookieCartData))
				{
					var cookieCartItems = JsonConvert.DeserializeObject<List<ShoppingCart>>(cookieCartData);
					shoppingCartVM.shoppingCarts = cookieCartItems;

					// Calculate totals
					foreach (var item in cookieCartItems)
					{
						shoppingCartVM.totalCarts += (item.Count * item.Product.Price);
					}
				}
			}
			// Add shipping cost
			shoppingCartVM.totalCartsWithShipping = shoppingCartVM.totalCarts + 50;

			return View(shoppingCartVM);
		}

		public IActionResult plus(int? cartid , Guid? guidid)
		{
			var claimsIdentity = User.Identity as ClaimsIdentity;
			var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
			if(claim != null)
			{
				var shoppingCart = _unitOfWork.ShoppingCart.GetFirstorDefault(x => x.shoppingId == cartid);
				_unitOfWork.ShoppingCart.IncreaseCount(shoppingCart, 1);
				_unitOfWork.Complete();
			}
            else
            {
                // User is not logged in; handle cookie-based cart
                var cookieCartData = HttpContext.Request.Cookies[SD.CartKey];
                if (!string.IsNullOrEmpty(cookieCartData))
                {
                    var cookieCartItems = JsonConvert.DeserializeObject<List<ShoppingCart>>(cookieCartData);
                    var item = cookieCartItems?.FirstOrDefault(x => x.shoppingIdGuid.Equals(guidid));

                    if (item != null)
                    {
                        item.Count += 1;
                        // Serialize the updated cart items list back to a cookie
                        var updatedCookieCartData = JsonConvert.SerializeObject(cookieCartItems);
                        HttpContext.Response.Cookies.Append(SD.CartKey, updatedCookieCartData, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            Expires = DateTimeOffset.Now.AddDays(7)
                        });
                    }
                }
            }
            return RedirectToAction("Index");
		}
		public IActionResult Minus(int? cartid, Guid? guidid)
		{
			var claimsIdentity = User.Identity as ClaimsIdentity;
			var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
			if (claim != null)
			{
				var shoppingCart = _unitOfWork.ShoppingCart.GetFirstorDefault(x => x.shoppingId == cartid);
				if (shoppingCart.Count <= 1)
				{
					_unitOfWork.ShoppingCart.Remove(shoppingCart);
					var count = _unitOfWork.ShoppingCart.GetAll(X => X.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count() - 1;
					HttpContext.Session.SetInt32(SD.SessionKey, count);
				}
				_unitOfWork.ShoppingCart.decreaseCount(shoppingCart, 1);
				_unitOfWork.Complete();
			}
			else
			{
				// User is not logged in; handle cookie-based cart
				var cookieCartData = HttpContext.Request.Cookies[SD.CartKey];
				if (!string.IsNullOrEmpty(cookieCartData))
				{
					var cookieCartItems = JsonConvert.DeserializeObject<List<ShoppingCart>>(cookieCartData);
					var item = cookieCartItems?.FirstOrDefault(x => x.shoppingIdGuid.Equals(guidid));

					if (item != null)
					{
						item.Count -= 1;
						// Serialize the updated cart items list back to a cookie
						var updatedCookieCartData = JsonConvert.SerializeObject(cookieCartItems);
						HttpContext.Response.Cookies.Append(SD.CartKey, updatedCookieCartData, new CookieOptions
						{
							HttpOnly = true,
							Secure = true,
							Expires = DateTimeOffset.Now.AddDays(7)
						});
					}
				}
			}
			return RedirectToAction("Index");
		}
		public IActionResult Remove(int? cartid, Guid? guidid, string returnUrl)
		{
			var claimsIdentity = User.Identity as ClaimsIdentity;
			var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

			if (claim != null)
			{
				// User is logged in
				if (cartid.HasValue)
				{
					var shoppingCart = _unitOfWork.ShoppingCart.GetFirstorDefault(x => x.shoppingId == cartid.Value);
					if (shoppingCart != null)
					{
						_unitOfWork.ShoppingCart.Remove(shoppingCart);
						_unitOfWork.Complete();
						var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count();
						HttpContext.Session.SetInt32(SD.SessionKey, count);
					}
				}
			}
			else
			{
				// User is not logged in; handle cookie-based cart
				var cookieCartData = HttpContext.Request.Cookies[SD.CartKey];
				if (!string.IsNullOrEmpty(cookieCartData))
				{
					var cookieCartItems = JsonConvert.DeserializeObject<List<ShoppingCart>>(cookieCartData);
					var item = cookieCartItems?.FirstOrDefault(x => x.shoppingIdGuid.Equals(guidid));

					if (item != null)
					{
						cookieCartItems.Remove(item);
						var updatedCookieCartData = JsonConvert.SerializeObject(cookieCartItems);
						HttpContext.Response.Cookies.Append(SD.CartKey, updatedCookieCartData, new CookieOptions
						{
							HttpOnly = true,
							Secure = true,
							Expires = DateTimeOffset.Now.AddDays(7)
						});
					}
				}
			}

			// Save and decode returnUrl in session
			var decodedReturnUrl = HttpUtility.UrlDecode(returnUrl);
			HttpContext.Session.SetString("ReturnUrl", decodedReturnUrl ?? "/");

			// Redirect to returnUrl
			return Redirect(HttpContext.Session.GetString("ReturnUrl") ?? "/");
		}

		[HttpGet]
		[Authorize]
		public IActionResult Checkout()
		{
			var claimsIdentity = User.Identity as ClaimsIdentity;
			var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
			shoppingCartVM = new ShoppingCartVM()
			{
				shoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value, IncludeWord: "Product"),
				OrderHeader = new()
			};
			shoppingCartVM.OrderHeader.applicationUser = _unitOfWork.ApplicationUser.GetFirstorDefault(X => X.Id == claim.Value);
			shoppingCartVM.OrderHeader.FirstName = shoppingCartVM.OrderHeader.applicationUser.FirstName;
			shoppingCartVM.OrderHeader.LastName = shoppingCartVM.OrderHeader.applicationUser.LastName;
			shoppingCartVM.OrderHeader.Email = shoppingCartVM.OrderHeader.applicationUser.Email;

			// Get Saved Past Transaction
			shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.applicationUser.PhoneNumber;
			shoppingCartVM.OrderHeader.AdditionalPhoneNumber = shoppingCartVM.OrderHeader.applicationUser.AdditionalPhoneNumber;
			shoppingCartVM.OrderHeader.Address = shoppingCartVM.OrderHeader.applicationUser.Address;
			shoppingCartVM.OrderHeader.Region = shoppingCartVM.OrderHeader.applicationUser.Region;
			shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.applicationUser.City;

			foreach (var item in shoppingCartVM.shoppingCarts)
			{
				shoppingCartVM.OrderHeader.totalPrice += (item.Count * item.Product.Price);
			}
			shoppingCartVM.totalCartsWithShipping = shoppingCartVM.OrderHeader.totalPrice + 50;

			return View(shoppingCartVM);
		}
		[HttpPost]
		[ActionName("Checkout")]
		[AutoValidateAntiforgeryToken]
		[Authorize]
		public IActionResult PostCheckout(ShoppingCartVM shoppingCartvm)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			shoppingCartvm.shoppingCarts = _unitOfWork.ShoppingCart
				.GetAll(x => x.ApplicationUserId == claim.Value, IncludeWord: "Product");

			shoppingCartvm.OrderHeader.orderStatus = SD.Pending;
			shoppingCartvm.OrderHeader.paymentStatus = SD.Pending;
			shoppingCartvm.OrderHeader.Downloader = false;
			shoppingCartvm.OrderHeader.orderDate = DateTime.Now;
			shoppingCartvm.OrderHeader.ApplicationUserId = claim.Value;

			var applicationUser = _unitOfWork.ApplicationUser.GetFirstorDefault(u => u.Id == claim.Value);
			// Update the user with the data from shoppingCartvm
			applicationUser.PhoneNumber = shoppingCartvm.OrderHeader.PhoneNumber;
			applicationUser.AdditionalPhoneNumber = shoppingCartvm.OrderHeader.AdditionalPhoneNumber;
			applicationUser.Address = shoppingCartvm.OrderHeader.Address;
			applicationUser.Region = shoppingCartvm.OrderHeader.Region;
			applicationUser.City = shoppingCartvm.OrderHeader.City;

			foreach (var item in shoppingCartvm.shoppingCarts)
			{
				shoppingCartvm.OrderHeader.totalPrice += (item.Count * item.Product.Price);
			}
			shoppingCartvm.totalCartsWithShipping = shoppingCartvm.OrderHeader.totalPrice + 50;

			_unitOfWork.OrderHeader.Add(shoppingCartvm.OrderHeader);
			_unitOfWork.Complete();

			foreach (var item in shoppingCartvm.shoppingCarts)
			{
				OrderDetails orderDetails = new OrderDetails()
				{
					productId = item.ProductId,
					OrderHeaderId = shoppingCartvm.OrderHeader.Id,
					price = item.Product.Price,
					Count = item.Count,
					Color = item.Color,
					Size = item.Size,
				};
				_unitOfWork.OrderDetails.Add(orderDetails);
				_unitOfWork.Complete();
			}
			_unitOfWork.ShoppingCart.RemoveRange(shoppingCartvm.shoppingCarts);
			_unitOfWork.Complete();
            HttpContext.Session.SetInt32(SD.SessionKey,
               _unitOfWork.ShoppingCart.GetAll(X => X.ApplicationUserId == claim.Value).ToList().Count());
            TempData["Order"] = "Thank you for Placed Order";
			return RedirectToAction("Index", "Home");
		}
	}
}
