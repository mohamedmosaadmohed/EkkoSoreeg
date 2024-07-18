using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            if (claimsIdentity == null)
            {
                return View(new ShoppingCartVM());
            }
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return View(new ShoppingCartVM());
            }

            var shoppingCartVM = new ShoppingCartVM()
            {
                shoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value, IncludeWord: "Product"),
                totalCarts = 0
            };

            foreach (var item in shoppingCartVM.shoppingCarts)
            {
                shoppingCartVM.totalCarts += (item.Count * item.Product.Price);
            }

            shoppingCartVM.totalCartsWithShipping = shoppingCartVM.totalCarts + 50;

            return View(shoppingCartVM);
        }
    }
}
