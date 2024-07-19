using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using EkkoSoreeg.Entities.Models;
using EkkoSoreeg.Utilities;

namespace EkkoSoreeg.Web.DataSeed
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Check if roles exist, if not, create them
            if (!await roleManager.RoleExistsAsync(SD.AdminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(SD.AdminRole));
            }
            if (!await roleManager.RoleExistsAsync(SD.EditorRole))
            {
                await roleManager.CreateAsync(new IdentityRole(SD.EditorRole));
            }
            if (!await roleManager.RoleExistsAsync(SD.CustomerRole))
            {
                await roleManager.CreateAsync(new IdentityRole(SD.CustomerRole));
            }

            // Ensure the default admin account exists
            var adminUser = await userManager.FindByEmailAsync("admin@gmail.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com"
                };
                await userManager.CreateAsync(adminUser, "Admin@12");
                await userManager.AddToRoleAsync(adminUser, SD.AdminRole);
            }
        }
    }
}
