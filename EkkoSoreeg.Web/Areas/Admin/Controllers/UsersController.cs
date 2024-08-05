using EkkoSoreeg.DataAccess.Data;
using EkkoSoreeg.Entities.Repositories;
using EkkoSoreeg.Entities.ViewModels;
using EkkoSoreeg.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EkkoSoreeg.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.AdminRole)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Get the current signed-in user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claims.Value;

            // Get users except the current signed-in user
            var users = _context.TbapplicationUser
                .Where(x => x.Id != userId)
                .Select(user => new UserWithRolesViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    LockoutEnd = user.LockoutEnd,
                    Roles = _context.UserRoles
                        .Where(ur => ur.UserId == user.Id)
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .ToList()
                })
                .ToList();

            return View(users);
        }

        public IActionResult lockUnlock(string? Id)
        {
            var user = _context.TbapplicationUser.FirstOrDefault(X => X.Id == Id);
            if (user == null)
                return NotFound();
            // To Close
            if (user.LockoutEnd == null | user.LockoutEnd < DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddYears(1);
            }
            // To Open
            else
            {
                user.LockoutEnd = DateTime.Now;
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Users", new { area = "Admin" });
        }
        public IActionResult Delete(string Id)
        {
            var user = _context.TbapplicationUser.FirstOrDefault(X => X.Id == Id);
            if (user == null)
                return NotFound();
            _context.TbapplicationUser.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("Index", "Users", new { area = "Admin" });
        }
    }
}
