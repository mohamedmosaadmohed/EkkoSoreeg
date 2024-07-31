using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Entities.ViewModels;
using EkkoSoreeg.Utilities;
using EkkoSoreeg.Web.DataSeed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace EkkoSoreeg.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
		private IUnitOfWork _unitOfWork;
		public HomeController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index(int ? page)
		{
			var pageNumber = page ?? 1;
			int pageSize = 8;
			var products = _unitOfWork.Product.GetAll(IncludeWord: "ProductImages").ToPagedList(pageNumber,pageSize);
			return View(products);
		}
		public IActionResult Details(int Id,int ? page)
		{
			var pageNumber = page ?? 1;
			int pageSize = 6;
			var product = _unitOfWork.Product.GetFirstorDefault(X => X.Id == Id, IncludeWord: "TbCatagory,ProductColorMappings.ProductColor,ProductSizeMappings.ProductSize,ProductImages");
			var relatedProducts = _unitOfWork.Product.GetAll(x => x.TbCatagory.Name == product.TbCatagory.Name && x.Id != Id, IncludeWord: "ProductColorMappings.ProductColor,ProductSizeMappings.ProductSize,ProductImages").ToPagedList(pageNumber, pageSize); ;
			ShoppingCart obj = new ShoppingCart()
			{
				ProductId = Id,
				Product = product,
				Count = 1,
				RelatedProducts = relatedProducts
			};
			return View(obj);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Details(ShoppingCart shoppingCart)
		{
			var productFromDb = _unitOfWork.Product.GetFirstorDefault(x => x.Id == shoppingCart.ProductId, IncludeWord: "ProductImages");

			// Determine if the user is logged in
			var claimsIdentity = User.Identity as ClaimsIdentity;
			var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
			shoppingCart.Product = productFromDb;

			if (productFromDb.OfferPrice != 0)
			{
				shoppingCart.Product.Price = productFromDb.OfferPrice;
			}

			if (claim != null)
			{
				// User is logged in
				shoppingCart.ApplicationUserId = claim.Value;
				var cartObj = _unitOfWork.ShoppingCart.GetFirstorDefault(
					u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId
					&& u.Color == shoppingCart.Color && u.Size == shoppingCart.Size
				);

				if (cartObj == null)
				{
					_unitOfWork.ShoppingCart.Add(shoppingCart);
					_unitOfWork.Complete();
				}
				else
				{
					_unitOfWork.ShoppingCart.IncreaseCount(cartObj, shoppingCart.Count);
					_unitOfWork.Complete();
				}

				HttpContext.Session.SetInt32(SD.SessionKey,
					_unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count());
			}
			else
			{
                // Get cart items from cookie or initialize a new list if none found
                var cartCookie = HttpContext.Request.Cookies[SD.CartKey];
                var cartItems = !string.IsNullOrEmpty(cartCookie) ?
                     JsonConvert.DeserializeObject<List<ShoppingCart>>(cartCookie) :
                    new List<ShoppingCart>();

                // Find if the item already exists in the cart
                var cartItem = cartItems.FirstOrDefault(x => x.ProductId == shoppingCart.ProductId
                                                             && x.Color == shoppingCart.Color
                                                             && x.Size == shoppingCart.Size);

                if (cartItem == null)
				{
                    shoppingCart.shoppingIdGuid = Guid.NewGuid();
                    cartItems.Add(shoppingCart);
                }
                        
                else
                    cartItem.Count += shoppingCart.Count;

                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                var cartJson = JsonConvert.SerializeObject(cartItems, settings);
                HttpContext.Response.Cookies.Append(SD.CartKey, cartJson, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax
                });
                HttpContext.Session.SetInt32(SD.SessionKey, cartJson.Count());
            }
            TempData["Order"] = "Added to Cart Successfully";
			return RedirectToAction("Details", "Home", new { area = "Customer", id = shoppingCart.ProductId });
		}
		public IActionResult AboutUs()
		{
			return View();
		}

		public IActionResult Wish()
		{
			List<Product> productList = new List<Product>();
			string productData = Request.Cookies[SD.WishKey];
			if (!string.IsNullOrEmpty(productData))
			{
				productList = JsonConvert.DeserializeObject<List<Product>>(productData);
			}
			return View(productList);
		}
		[HttpGet]
        public IActionResult AddWishList(int Id)
        {
			var product = _unitOfWork.Product.GetFirstorDefault(X => X.Id == Id, IncludeWord:
				"TbCatagory,ProductColorMappings.ProductColor,ProductSizeMappings.ProductSize,ProductImages");
			List<Product> productList = new List<Product>();
            var productData = HttpContext.Request.Cookies[SD.WishKey];
			productList?.Add(product);
			var settings = new JsonSerializerSettings
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};
			string updatedCartData = JsonConvert.SerializeObject(productList, settings);
			Response.Cookies.Append(SD.WishKey, updatedCartData, new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				Expires = DateTimeOffset.UtcNow.AddDays(7)
			});
			return RedirectToAction("Wish", "Home");
        }
		public IActionResult DeleteFromWish(int wishid)
		{
			var productData = HttpContext.Request.Cookies[SD.WishKey];
			if (!string.IsNullOrEmpty(productData))
			{
				var wishCartList = JsonConvert.DeserializeObject<List<Product>>(productData);
				var item = wishCartList?.FirstOrDefault(X => X.Id == wishid);
				if (item != null)
				{
					wishCartList?.Remove(item);
					var updatedCookieWishData = JsonConvert.SerializeObject(wishCartList);
					HttpContext.Response.Cookies.Append(SD.WishKey, updatedCookieWishData, new CookieOptions
					{
						HttpOnly = true,
						Secure = true,
						Expires = DateTimeOffset.Now.AddDays(7)
					});
				}
			}
			return RedirectToAction("Wish", "Home");
		}
	}
}
