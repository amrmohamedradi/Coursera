using Coursera.Application.Common.Constans;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Infrastructure.Identity
{
    public class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole<Guid>> roleManager,UserManager<ApplicationUser> userManager)
        {
            var roles = new[] { Roles.Admin, Roles.User};
            foreach(var role in roles)
            {
                if(!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            var adminEmail = "admin@coursera.com";
            var adminUserName = "admin";
            var existingAdmin = await userManager.FindByNameAsync(adminUserName);
            if (existingAdmin != null) 
                return;
            
                var admin = new ApplicationUser("System","Admin","admin",adminEmail);
                var result = await userManager.CreateAsync(admin, "Admin@Admin0");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, Roles.Admin);
        }
    }
}
