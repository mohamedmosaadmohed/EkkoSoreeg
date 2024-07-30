using Azure.Core;
using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace EkkoSoreeg.Web.ViewComponents
{
	public class WishViewComponent : ViewComponent
	{

		public async Task<IViewComponentResult> InvokeAsync()
		{

			string wishData = Request.Cookies[SD.WishKey];
			int wishCount = 0;

			if (!string.IsNullOrEmpty(wishData))
			{
				var wishList = JsonConvert.DeserializeObject<List<Product>>(wishData);
				wishCount = wishList.Count;
			}
			return View(wishCount);
		}
	}
}
