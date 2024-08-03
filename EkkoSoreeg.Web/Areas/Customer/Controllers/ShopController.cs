using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace EkkoSoreeg.Web.Areas.Customer.Controllers
{
	[Area("Customer")]
    public class ShopController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public ShopController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int? page, string searchQuery, string category, string sortBy, string color, decimal? minPrice, decimal? maxPrice)
        {
            var pageNumber = page ?? 1;
            int pageSize = 4;

            var products = _unitOfWork.Product.GetAll(IncludeWord: "TbCatagory,ProductImages,ProductColorMappings.ProductColor,ProductSizeMappings.ProductSize");
            var categories = _unitOfWork.Catagory.GetAll();
            var colors = _unitOfWork.Color.GetAll();

            // Search query filter
            if (!string.IsNullOrEmpty(searchQuery))
            {
                products = products.Where(p => p.Name.Contains(searchQuery) || p.Description.Contains(searchQuery));
            }

            // Category filter
            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                products = products.Where(p => p.TbCatagory != null && p.TbCatagory.Name.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            // Color filter
            if (!string.IsNullOrEmpty(color))
            {
                products = products.Where(p => p.ProductColorMappings != null && p.ProductColorMappings.Any(pc => pc.ProductColor != null && pc.ProductColor.Name.Equals(color, StringComparison.OrdinalIgnoreCase)));
            }

            // Price range filter
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            // Sorting
            switch (sortBy)
            {
                case "Offers":
                    products = products.Where(p => p.OfferPrice > 0 &&  p.Stock != 0);
                    break;
                case "Newness":
                    products = products.OrderByDescending(p => p.CreateDate);
                    break;
                case "PriceLowToHigh":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "PriceHighToLow":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.Id);
                    break;
            }

            // Pagination
            var pagedProducts = products.ToPagedList(pageNumber, pageSize);

            var viewModel = new ShopViewModel
            {
                Products = pagedProducts,
                Categories = categories.ToList(),
                Colors = colors.ToList(),
                SelectedCategory = category,
            };

            return View(viewModel);
        }
    }



}
