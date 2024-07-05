using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Entities.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EkkoSoreeg.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CatagoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CatagoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var catagory = _unitOfWork.Catagory.GetAll();
            return View(catagory);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(Catagory catagory)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Catagory.Add(catagory);
                _unitOfWork.Complete();
                TempData["Create"] = "Catagory Has been Created Successfully";
                return RedirectToAction("Index");
            }
            return View(catagory);

        }

        public IActionResult Update(int? Id)
        {
            if (Id == null || Id == 0)
                NotFound();
            var item = _unitOfWork.Catagory.GetFirstorDefault(x => x.Id == Id);
            return View(item);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Update(Catagory catagory)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Catagory.Update(catagory);
                _unitOfWork.Complete();
                TempData["Update"] = "Catagory Has been Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(catagory);
        }
        [HttpDelete]
        public IActionResult Delete(int? Id)
        {
            var item = _unitOfWork.Catagory.GetFirstorDefault(x => x.Id == Id);
            if (item == null)
                return NotFound();

            _unitOfWork.Catagory.Remove(item);
            _unitOfWork.Complete();
            // TempData["Delete"] = "Category has been deleted successfully.";
            return Json(new { success = true, message = "Product Has been Deleted Successfully" }); // (Sweetalert)
        }

    }
}
