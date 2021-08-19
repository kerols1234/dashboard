using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace dashboard.Models
{
    public class Employee
    {
        public int Id { set; get; }
        [Required]
        public int Code { get; set; }
        [Required]
        public string EnglishName { set; get; }
        [Required]
        public string ArabicName { set; get; }
        [Required]
        public string JobTitle { set; get; }
        [ForeignKey("Department")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        public virtual Department Department { set; get; }
        [Required]
        public string Email { set; get; }
        public Boolean Insurance { get; set; }
    }
}