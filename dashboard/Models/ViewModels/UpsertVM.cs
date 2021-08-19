using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dashboard.Models
{
    public class UpsertVM
    {
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string Name { get; set; }

        [Display(Name = "Phone Number")]
        [StringLength(11,ErrorMessage = "The must be at least characters long.", MinimumLength = 11)]
        [Phone]
        public string PhoneNumber { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        public IEnumerable<SelectListItem> UserSelectList { get; set; }
    }
}
