using Microsoft.AspNetCore.Identity;
using RentACarSystem.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentACarSystem.Data
{
    public static class ApplicationDbInitializer
    {
        public static void SeedUsers(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedAdmin(userManager);
            SeedClient(userManager);
        }

        static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (roleManager.FindByNameAsync("Admin").Result == null)
            {
                roleManager.CreateAsync(new IdentityRole()
                {
                    Name = "Admin"
                }).Wait();
            }

            if (roleManager.FindByNameAsync("Client").Result == null)
            {
                roleManager.CreateAsync(new IdentityRole()
                {
                    Name = "Client"
                }).Wait();
            }
        }

        static void SeedAdmin(UserManager<AppUser> userManager)
        {
            if (userManager.FindByNameAsync("admin").Result == null)
            {
                AppUser user = new AppUser
                {
                    UserName = "admin",
                    Email = "admin@abv.bg",
                    FirstName = "Admin",
                    MiddleName = "Admin",
                    LastName = "Admin",
                    EGN = "1234567890"
                    //EmailConfirmed = true
                };

                IdentityResult result = userManager.CreateAsync(user, "adminpass").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }

        static void SeedClient(UserManager<AppUser> userManager)
        {
            if (userManager.FindByNameAsync("client").Result == null)
            {
                AppUser user = new AppUser
                {
                    UserName = "client",
                    Email = "client@abv.bg",
                    FirstName = "Client",
                    MiddleName = "Client",
                    LastName = "Client",
                    EGN = "0987654321"
                    //EmailConfirmed = true
                };

                IdentityResult result = userManager.CreateAsync(user, "clientpass").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Client").Wait();
                }
            }
        }
    }
}
