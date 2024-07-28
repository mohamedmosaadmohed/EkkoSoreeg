using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EkkoSoreeg.Web.ViewComponents
{
	public class ModalViewComponent : ViewComponent
	{
		private IUnitOfWork _unitOfWork;
		public ModalViewComponent(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public async Task<IViewComponentResult> InvokeAsync(int Id)
		{
			var product = _unitOfWork.Product.GetFirstorDefault(
				x => x.Id == Id,
				IncludeWord: "TbCatagory,ProductColorMappings.ProductColor,ProductSizeMappings.ProductSize,ProductImages"
			  );
			var obj = new ShoppingCart
			{
				ProductId = Id,
				Product = product,
				Count = 1
			};
			return View(obj);
		}
	}
}
