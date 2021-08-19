using dashboard.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationIdentity.Models;

namespace dashboard.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<Department> departments { get; set; }
        public DbSet<Employee> employees { get; set; }


    }
}
