using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EkkoSoreeg.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole + "," + SD.EditorRole)]
    public class SizeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public SizeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var size = _unitOfWork.Size.GetAll();
            return View(size);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(ProductSize productSize)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Size.Add(productSize);
                _unitOfWork.Complete();
                TempData["Create"] = "Size Has been Created Successfully";
                return RedirectToAction("Index");
            }
            return View(productSize);

        }

        public IActionResult Update(int? Id)
        {
            if (Id == null || Id == 0)
                NotFound();
            var item = _unitOfWork.Size.GetFirstorDefault(x => x.Id == Id);
            return View(item);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Update(ProductSize productSize)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Size.Update(productSize);
                _unitOfWork.Complete();
                TempData["Update"] = "Size Has been Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(productSize);
        }
        [HttpDelete]
        public IActionResult Delete(int? Id)
        {
            var item = _unitOfWork.Size.GetFirstorDefault(x => x.Id == Id);
            if (item == null)
                return NotFound();

            _unitOfWork.Size.Remove(item);
            _unitOfWork.Complete();
            return Json(new { success = true, message = "Size Has been Deleted Successfully" }); // (Sweetalert)
        }

    }
}
