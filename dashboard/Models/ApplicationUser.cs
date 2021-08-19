using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationIdentity.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }


    }
}
