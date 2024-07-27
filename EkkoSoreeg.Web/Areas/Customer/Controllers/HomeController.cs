using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
			int pageSize = 6;
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
		[Authorize]
		public IActionResult Details(ShoppingCart shoppingCart)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			shoppingCart.ApplicationUserId = claim.Value;
			ShoppingCart cartObj = _unitOfWork.ShoppingCart.GetFirstorDefault(
				U => U.ApplicationUserId == claim.Value && U.ProductId == shoppingCart.ProductId
				&& U.Color == shoppingCart.Color && U.Size == shoppingCart.Size
			);
			if (cartObj == null)
			{
				_unitOfWork.ShoppingCart.Add(shoppingCart);
				_unitOfWork.Complete();
				HttpContext.Session.SetInt32(SD.SessionKey,
					_unitOfWork.ShoppingCart.GetAll(X => X.ApplicationUserId == claim.Value).ToList().Count());

			}
			else
			{
				_unitOfWork.ShoppingCart.IncreaseCount(cartObj, shoppingCart.Count);
				_unitOfWork.Complete();
			}
			
			return RedirectToAction("Index");
		}

		public IActionResult AboutUs()
		{
			return View();
		}
	}
}
