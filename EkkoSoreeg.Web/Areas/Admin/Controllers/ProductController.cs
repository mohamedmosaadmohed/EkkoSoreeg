using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EkkoSoreeg.Entities.ViewModels;

namespace EkkoSoreeg.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;

        }
        [HttpGet]
        public IActionResult GetData()
        {
            var products = _unitOfWork.Product.GetAll(IncludeWord: "TbCatagory");
            return Json(new { data = products });
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
                })
            };
            return View(productVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(ProductVM Productvm, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(rootPath, @"Dashboard\Images\Products");
                    var extention = Path.GetExtension(file.FileName); // .JPG
                    using (var fileStream = new FileStream(Path.Combine(Upload, filename + extention), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    Productvm.Product.Image = @"Dashboard\Images\Products\" + filename + extention;
                }
                _unitOfWork.Product.Add(Productvm.Product);
                _unitOfWork.Complete();
                TempData["Create"] = "Product Has been Created Successfully";
                return RedirectToAction("Index");
            }
            return View(Productvm.Product);

        }
        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null | id == 0)
                NotFound();

            ProductVM productVM = new ProductVM()
            {
                Product = _unitOfWork.Product.GetFirstorDefault(x => x.Id == id),
                CatagoryList = _unitOfWork.Catagory.GetAll().Select(X => new SelectListItem
                {
                    Text = X.Name,
                    Value = X.Id.ToString()
                })
            };
            return View(productVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Update(ProductVM Productvm, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(rootPath, @"Dashboard\Images\Products");
                    var extention = Path.GetExtension(file.FileName);
                    if (Productvm.Product.Image != null)
                    {
                        var oldimg = Path.Combine(rootPath, Productvm.Product.Image.TrimStart('\\'));
                        if (System.IO.File.Exists(oldimg))
                        {
                            System.IO.File.Delete(oldimg);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(Upload, filename + extention), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    Productvm.Product.Image = @"Dashboard\Images\Products" + filename + extention;
                }
                _unitOfWork.Product.Update(Productvm.Product);
                _unitOfWork.Complete();
                TempData["Update"] = "Product Has been Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(Productvm.Product);
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
