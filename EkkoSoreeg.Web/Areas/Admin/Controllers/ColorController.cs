using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace EkkoSoreeg.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class ColorController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ColorController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var colors = _unitOfWork.Color.GetAll();
            return View(colors);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(ProductColor productColor)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Color.Add(productColor);
                _unitOfWork.Complete();
                TempData["Create"] = "Color Has been Created Successfully";
                return RedirectToAction("Index");
            }
            return View(productColor);

        }

        public IActionResult Update(int? Id)
        {
            if (Id == null || Id == 0)
                NotFound();
            var item = _unitOfWork.Color.GetFirstorDefault(x => x.Id == Id);
            return View(item);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Update(ProductColor productColor)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Color.Update(productColor);
                _unitOfWork.Complete();
                TempData["Update"] = "Color Has been Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(productColor);
        }
        [HttpDelete]
        public IActionResult Delete(int? Id)
        {
            var item = _unitOfWork.Color.GetFirstorDefault(x => x.Id == Id);
            if (item == null)
                return NotFound();

            _unitOfWork.Color.Remove(item);
            _unitOfWork.Complete();
            return Json(new { success = true, message = "Color Has been Deleted Successfully" }); // (Sweetalert)
        }

    }
}
