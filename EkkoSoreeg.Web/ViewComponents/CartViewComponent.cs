using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Entities.ViewModels;
using EkkoSoreeg.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EkkoSoreeg.Web.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            var shoppingCartVM = new ShoppingCartVM
            {
                totalCarts = 0,
                totalCartsWithShipping = 0,
                shoppingCarts = new List<ShoppingCart>()
            };

            if (claim != null)
            {
                // User is logged in
                shoppingCartVM.shoppingCarts = _unitOfWork.ShoppingCart.GetAll(x =>
                    x.ApplicationUserId == claim.Value, IncludeWord: "Product,Product.ProductImages").ToList();

                foreach (var item in shoppingCartVM.shoppingCarts)
                {
                    if (item.Product != null)
                    {
                        shoppingCartVM.totalCarts += (item.Count * item.Product.Price);
                    }
                }

                shoppingCartVM.totalCartsWithShipping = shoppingCartVM.totalCarts + 50;
            }
            else
            {
                // User is not logged in; handle cookie-based cart
                var cookieCartData = HttpContext.Request.Cookies[SD.CartKey];
                if (!string.IsNullOrEmpty(cookieCartData))
                {
                    var cookieCartItems = JsonConvert.DeserializeObject<List<ShoppingCart>>(cookieCartData);

                    // You might want to validate if cookieCartItems is null or empty
                    if (cookieCartItems != null)
                    {
                        shoppingCartVM.shoppingCarts = cookieCartItems;

                        foreach (var item in shoppingCartVM.shoppingCarts)
                        {
                            if (item.Product != null)
                            {
                                shoppingCartVM.totalCarts += (item.Count * item.Product.Price);
                            }
                        }

                        shoppingCartVM.totalCartsWithShipping = shoppingCartVM.totalCarts + 50;
                    }
                }
            }

            return View(shoppingCartVM);
        }


    }
}
