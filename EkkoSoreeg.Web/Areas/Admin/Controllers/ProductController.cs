using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EkkoSoreeg.Entities.ViewModels;
using EkkoSoreeg.Utilities;
using Microsoft.AspNetCore.Authorization;
using EkkoSoreeg.DataAccess.Data;
using System.Drawing;
using Microsoft.CodeAnalysis;

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
                // Add product

				_unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Complete();
                // Add Image in Folder
				string rootPath = _webHostEnvironment.WebRootPath;
				List<string> imagePaths = new List<string>();
				if (files != null && files.Count > 0)
				{
					// Create a unique folder for the product
					string productFolder = Path.Combine(rootPath, @"Dashboard\Images\Products", productVM.Product.Id.ToString());
					if (!Directory.Exists(productFolder))
					{
						Directory.CreateDirectory(productFolder);
					}

					foreach (var file in files)
					{
						string filename = Guid.NewGuid().ToString();
						var extension = Path.GetExtension(file.FileName);
						var filePath = Path.Combine(productFolder, filename + extension);

						using (var fileStream = new FileStream(filePath, FileMode.Create))
						{
							file.CopyTo(fileStream);
						}

						// Save relative path to database
						string relativePath = Path.Combine(@"Dashboard\Images\Products", productVM.Product.Id.ToString(), filename + extension);
						imagePaths.Add(relativePath);
					}
				}
                // Add product image mappings
                foreach (var imagePath in imagePaths)
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
			TempData["Delete"] = "Faild to Create";
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
                ImageList = _context.ProductImages.Select(X => new SelectListItem
                {
                    Text = X.ImagePath,
                    Value = X.Id.ToString()
                }),
                SelectedColors = selectedColors,
                SelectedSizes = selectedSizes
            };
            return View(productVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(ProductVM productVM, IFormFile[] files)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;

                // Remove all existing images for the product
                var existingImages = _context.ProductImages.Where(img => img.ProductId == productVM.Product.Id).ToList();
                foreach (var image in existingImages)
                {
                    var imagePath = Path.Combine(rootPath, image.ImagePath.TrimStart('\\'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                _context.ProductImages.RemoveRange(existingImages);
                await _context.SaveChangesAsync();

                // Upload new images
                if (files != null && files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            string filename = Guid.NewGuid().ToString();
                            var uploadPath = Path.Combine(rootPath, $"Dashboard\\Images\\Products\\{productVM.Product.Id.ToString()}");
                            var extension = Path.GetExtension(file.FileName);

                            // Create directory if it doesn't exist
                            if (!Directory.Exists(uploadPath))
                            {
                                Directory.CreateDirectory(uploadPath);
                            }

                            // Save the new image
                            var filePath = Path.Combine(uploadPath, filename + extension);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                            }

                            // Add image record to the database
                            var newImage = new ProductImage
                            {
                                ProductId = productVM.Product.Id,
                                ImagePath = $"Dashboard\\Images\\Products\\{productVM.Product.Id.ToString()}\\" + filename + extension
                            };
                           await _context.ProductImages.AddAsync(newImage);
                        }
                    }
                    await _context.SaveChangesAsync();
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
            return View(productVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult UpdateImage(int? imageId ,int productId, IFormFile file)
        {
            string rootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                var productImage = _context.ProductImages.Find(imageId);
                if (productImage != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var uploadPath = Path.Combine(rootPath, $"Dashboard\\Images\\Products\\{productId}");
                    var extension = Path.GetExtension(file.FileName);

                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(productImage.ImagePath))
                    {
                        var oldImagePath = Path.Combine(rootPath, productImage.ImagePath.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save the new image
                    using (var fileStream = new FileStream(Path.Combine(uploadPath, filename + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productImage.ImagePath = $"Dashboard\\Images\\Products\\{productId}\\" + filename + extension;
                    _context.SaveChanges();

                    TempData["Update"] = "Product image has been updated successfully";
                }
            }
            return  RedirectToAction("Update", "Product", new { id = productId });
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult AddImages(int productId, IFormFile[] files)
        {
            string rootPath = _webHostEnvironment.WebRootPath;
            var uploadPath = Path.Combine(rootPath, $"Dashboard\\Images\\Products\\{productId}");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            foreach (var file in files)
            {
                if (file != null && file.Length > 0)
                {
                    string filename = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(file.FileName);

                    // Save the new image
                    using (var fileStream = new FileStream(Path.Combine(uploadPath, filename + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    var productImage = new ProductImage
                    {
                        ProductId = productId,
                        ImagePath = $"Dashboard\\Images\\Products\\{productId}\\{filename}{extension}"
                    };
                    _context.ProductImages.Add(productImage);
                }
            }
            _context.SaveChanges();
            TempData["Update"] = "Images have been added successfully";
            return RedirectToAction("Update", new { id = productId });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int? Id)
        {
            var item = _unitOfWork.Product.GetFirstorDefault(x => x.Id == Id);
            if (item == null)
                return Json(new { success = false, message = "Error While Deleting" });

            // Remove the product from the database
            _unitOfWork.Product.Remove(item);
            _unitOfWork.Complete();

            string rootPath = _webHostEnvironment.WebRootPath;
            var productImagePath = Path.Combine(rootPath, $"Dashboard\\Images\\Products\\{Id}");

            // Delete all images associated with the product
            var existingImages = _context.ProductImages.Where(img => img.ProductId == Id).ToList();
            foreach (var image in existingImages)
            {
                var imagePath = Path.Combine(rootPath, image.ImagePath.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            // Remove image records from the database
            _context.ProductImages.RemoveRange(existingImages);
            await _context.SaveChangesAsync();

            // Delete all files in the product's directory
            if (Directory.Exists(productImagePath))
            {
                var files = Directory.GetFiles(productImagePath);
                foreach (var file in files)
                {
                    System.IO.File.Delete(file);
                }
                Directory.Delete(productImagePath); // Delete the directory if it's empty
            }

            return Json(new { success = true, message = "Product Has been Deleted Successfully" });
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteImage(int ? imageId,int productId)
        {
            var productImage = await _context.ProductImages.FindAsync(imageId);
            if (productImage == null)
                return Json(new { success = false, message = "Error While Deleting" });
            string rootPath = _webHostEnvironment.WebRootPath;
            var uploadPath = Path.Combine(rootPath, $"Dashboard\\Images\\Products\\{productId}");

            // Delete the image if it exists
            if (!string.IsNullOrEmpty(productImage.ImagePath))
            {
                var ImagePath = Path.Combine(rootPath, productImage.ImagePath.TrimStart('\\'));
                if (System.IO.File.Exists(ImagePath))
                {
                    System.IO.File.Delete(ImagePath);
                }
            }
            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Product Has been Deleted Successfully" });
        }

    }
}
