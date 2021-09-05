using dashboard.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationIdentity.Models;

namespace dashboard.Models
{
    public class SampleData
    {
        private readonly ApplicationDbContext context;
        public SampleData(ApplicationDbContext db)
        {
            context = db;
        }
        public void Initialize()
        {

            var user = new ApplicationUser
            {
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                PhoneNumber = "01554860303",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                EmployeeName = "admin"
            };


            if (!context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, "admin");
                user.PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser>(context);
                var result = userStore.CreateAsync(user);

            }

            context.SaveChangesAsync();
        }

    }
}
