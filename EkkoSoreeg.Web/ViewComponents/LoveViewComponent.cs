using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.ViewModels;
using EkkoSoreeg.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace EkkoSoreeg.Web.ViewComponents
{
	public class LoveViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int wishid)
        {
            var productData = HttpContext.Request.Cookies[SD.WishKey];
            bool isInWishlist = false;

            if (!string.IsNullOrEmpty(productData))
            {
                var wishCartList = JsonConvert.DeserializeObject<List<Product>>(productData);
                var item = wishCartList?.FirstOrDefault(x => x.Id == wishid);
                isInWishlist = item != null;
            }

            return View(isInWishlist);
        }
    }

}
