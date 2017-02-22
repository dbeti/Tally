using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tally.Models;

namespace Tally.Data
{
    public static class Data
    {
        private static readonly string[] Roles = new string[] { "Professor", "Student" };

        public static async Task Seed(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var db = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService <UserManager<ApplicationUser>>();

                foreach (var role in Roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                var user = new ApplicationUser
                {
                    FirstName = "Profesor",
                    LastName = "1",
                    Email = "profesor1@pmf.hr",
                    UserName = "profesor1@pmf.hr",
                    NormalizedUserName = "PROFESOR1@PMF.HR",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };

                await CreateUser(db, serviceProvider, user, "Professor");

                user = new ApplicationUser
                {
                    FirstName = "Profesor",
                    LastName = "2",
                    Email = "profesor2@pmf.hr",
                    UserName = "profesor2@pmf.hr",
                    NormalizedUserName = "PROFESOR2@PMF.HR",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };

                await CreateUser(db, serviceProvider,user, "Professor");

                user = new ApplicationUser
                {
                    FirstName = "Student",
                    LastName = "1",
                    Email = "student1@pmf.hr",
                    UserName = "student1@pmf.hr",
                    NormalizedUserName = "STUDENT1@PMF.HR",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };

                await CreateUser(db, serviceProvider, user, "Student");

                user = new ApplicationUser
                {
                    FirstName = "Student",
                    LastName = "2",
                    Email = "student2@pmf.hr",
                    UserName = "student2@pmf.hr",
                    NormalizedUserName = "STUDENT2@PMF.HR",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };

                await CreateUser(db, serviceProvider, user, "Student");

                user = new ApplicationUser
                {
                    FirstName = "Student",
                    LastName = "3",
                    Email = "student3@pmf.hr",
                    UserName = "student3@pmf.hr",
                    NormalizedUserName = "STUDENT3@PMF.HR",
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };

                await CreateUser(db, serviceProvider, user, "Student");




            }
        }

        private static async Task<IdentityResult> CreateUser(ApplicationDbContext db, IServiceProvider services, ApplicationUser user, string role)
        {
            if (!db.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, "Test1234");
                user.PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser>(db);
                await userStore.CreateAsync(user);

                UserManager<ApplicationUser> _userManager = services.GetService<UserManager<ApplicationUser>>();
                ApplicationUser created = await _userManager.FindByNameAsync(user.UserName);
                return await _userManager.AddToRoleAsync(created, role);
            }
            else
            {
                return IdentityResult.Success;
            }
        }
    }
}
