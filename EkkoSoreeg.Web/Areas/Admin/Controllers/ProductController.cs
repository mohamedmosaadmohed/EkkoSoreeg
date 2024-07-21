using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EkkoSoreeg.Entities.ViewModels;
using EkkoSoreeg.Utilities;
using Microsoft.AspNetCore.Authorization;
using EkkoSoreeg.DataAccess.Data;
using System.Drawing;

namespace EkkoSoreeg.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment , ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _context = context;

        }
        public IActionResult GetData()
        {
            var products = _unitOfWork.Product.GetAll(IncludeWord: "TbCatagory,ProductColorMappings.ProductColor,ProductSizeMappings.ProductSize");
            var productList = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Stock,
                p.StockType,
                p.Price,
                p.OfferPrice,
                p.CreateDate,
                TbCatagory = new { p.TbCatagory.Id, p.TbCatagory.Name, p.TbCatagory.Description, p.TbCatagory.CreateDate },
                ProductColors = p.ProductColorMappings.Select(c => c.ProductColor.Name).ToList(),
                ProductSizes = p.ProductSizeMappings.Select(s => s.ProductSize.Name).ToList()
            }).ToList();

            return Json(new { data = productList });
        }



        public IActionResult Index()
        {
            var Product = _unitOfWork.Product.GetAll();
            return View(Product);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CatagoryList = _unitOfWork.Catagory.GetAll().Select(X => new SelectListItem
                {
                    Text = X.Name,
                    Value = X.Id.ToString()
                }),
                ColorList = _unitOfWork.Color.GetAll().Select(X => new SelectListItem
                {
                    Text = X.Name,
                    Value = X.Id.ToString()
                }),
                SizeList = _unitOfWork.Size.GetAll().Select(X => new SelectListItem
                {
                    Text = X.Name,
                    Value = X.Id.ToString()
                }),
            };

            return View(productVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(ProductVM productVM, List<IFormFile> files, List<int> SelectedColors, List<int> SelectedSizes)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                List<string> imagePaths = new List<string>();

                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        string filename = Guid.NewGuid().ToString();
                        var uploadPath = Path.Combine(rootPath, @"Dashboard\Images\Products");
                        var extension = Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(uploadPath, filename + extension);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        imagePaths.Add(@"Dashboard\Images\Products\" + filename + extension);
                    }

                    // Store the first image as the main product image
                    productVM.Product.Image = imagePaths.First();
                }

                // Add product
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Complete();

                // Add product image mappings
                foreach (var imagePath in imagePaths.Skip(1))
                {
                    var productImage = new ProductImage
                    {
                        ProductId = productVM.Product.Id,
                        ImagePath = imagePath
                    };
                   await _context.ProductImages.AddAsync(productImage);
                }

                // Add selected colors
                foreach (var colorId in SelectedColors)
                {
                    var productColorMapping = new ProductColorMapping
                    {
                        ProductId = productVM.Product.Id,
                        ProductColorId = colorId
                    };
                   await _context.ProductColorMappings.AddAsync(productColorMapping);
                }

                // Add selected sizes
                foreach (var sizeId in SelectedSizes)
                {
                    var productSizeMapping = new ProductSizeMapping
                    {
                        ProductId = productVM.Product.Id,
                        ProductSizeId = sizeId
                    };
                    await _context.ProductSizeMappings.AddAsync(productSizeMapping);
                }

                _unitOfWork.Complete();
                TempData["Create"] = "Product Has been Created Successfully";
                return RedirectToAction("Index");
            }
            return View(productVM);
        }
        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null | id == 0)
                NotFound();
            var selectedColors = _context.ProductColorMappings
                                  .Where(x => x.ProductId == id)
                                  .Select(x => x.ProductColorId)
                                  .ToList();

            var selectedSizes = _context.ProductSizeMappings
                                        .Where(x => x.ProductId == id)
                                        .Select(x => x.ProductSizeId)
                                        .ToList();
            ProductVM productVM = new ProductVM()
            {
                Product = _unitOfWork.Product.GetFirstorDefault(x => x.Id == id),
                CatagoryList = _unitOfWork.Catagory.GetAll().Select(X => new SelectListItem
                {
                    Text = X.Name,
                    Value = X.Id.ToString()
                }),
                ColorList = _unitOfWork.Color.GetAll().Select(X => new SelectListItem
                {
                    Text = X.Name,
                    Value = X.Id.ToString()
                }),
                SizeList = _unitOfWork.Size.GetAll().Select(X => new SelectListItem
                {
                    Text = X.Name,
                    Value = X.Id.ToString()
                }),
                SelectedColors = selectedColors,
                SelectedSizes = selectedSizes
            };
            return View(productVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Update(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var uploadPath = Path.Combine(rootPath, @"Dashboard\Images\Products");
                    var extension = Path.GetExtension(file.FileName);
                    if (productVM.Product.Image != null)
                    {
                        var oldImage = Path.Combine(rootPath, productVM.Product.Image.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImage))
                        {
                            System.IO.File.Delete(oldImage);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(uploadPath, filename + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.Image = @"Dashboard\Images\Products\" + filename + extension;
                }

                _unitOfWork.Product.Update(productVM.Product);

                // Retrieve existing color mappings for the product
                var existingColorMappings = _context.ProductColorMappings
                    .Where(pcm => pcm.ProductId == productVM.Product.Id)
                    .ToList();
                // Retrieve existing Size mappings for the product
                var existingSizeMappings = _context.ProductSizeMappings
                    .Where(pcm => pcm.ProductId == productVM.Product.Id)
                    .ToList();

                // Remove mappings that are not in the selected colors
                var removedMappingsColor = existingColorMappings
                    .Where(pcm => !productVM.SelectedColors.Contains(pcm.ProductColorId))
                    .ToList();
                // Remove mappings that are not in the selected Sizes
                var removedMappingsSize = existingSizeMappings
                    .Where(pcm => !productVM.SelectedSizes.Contains(pcm.ProductSizeId))
                    .ToList();

                _context.ProductColorMappings.RemoveRange(removedMappingsColor);
                _context.ProductSizeMappings.RemoveRange(removedMappingsSize);

                // Add or update mappings based on the selected colors
                foreach (var colorId in productVM.SelectedColors)
                {
                    var existingMapping = existingColorMappings
                        .FirstOrDefault(pcm => pcm.ProductColorId == colorId);

                    if (existingMapping == null)
                    {
                        // Add new mapping if it doesn't exist
                        var newMapping = new ProductColorMapping
                        {
                            ProductId = productVM.Product.Id,
                            ProductColorId = colorId
                        };
                        _context.ProductColorMappings.Add(newMapping);
                    }
                    // If it exists, no need to update since it's already present
                }


                // Add or update mappings based on the selected Sizes
                foreach (var sizeId in productVM.SelectedSizes)
                {
                    var existingMapping = existingSizeMappings
                        .FirstOrDefault(pcm => pcm.ProductSizeId == sizeId);

                    if (existingMapping == null)
                    {
                        // Add new mapping if it doesn't exist
                        var newMapping = new ProductSizeMapping
                        {
                            ProductId = productVM.Product.Id,
                            ProductSizeId = sizeId
                        };
                        _context.ProductSizeMappings.Add(newMapping);
                    }
                    // If it exists, no need to update since it's already present
                }
                _unitOfWork.Complete();

                TempData["Update"] = "Product Has been Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(productVM.Product);
        }

        [HttpDelete]
        public IActionResult DeleteProduct(int? Id)
        {
            var item = _unitOfWork.Product.GetFirstorDefault(x => x.Id == Id);
            if (item == null)
                return Json(new { success = false, message = "Error While Deleting" });
            _unitOfWork.Product.Remove(item);
            var oldimg = Path.Combine(_webHostEnvironment.WebRootPath, item.Image.TrimStart('\\'));
            if (System.IO.File.Exists(oldimg))
            {
                System.IO.File.Delete(oldimg);
            }
            _unitOfWork.Complete();
            //TempData["Delete"] = "Product Has been Deleted Successfully"; // (Toaster)
            return Json(new { success = true, message = "Product Has been Deleted Successfully" }); // (Sweetalert)
        }

    }
}
