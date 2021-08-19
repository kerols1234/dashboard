using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dashboard.Models.ViewModels
{
    public class EmployeeVM
    {
        public Employee Employee { get; set; }
        public IEnumerable<SelectListItem> EmployeeSelectList { get; set; }
    }
}
