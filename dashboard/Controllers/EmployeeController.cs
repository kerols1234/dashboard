using dashboard.Data;
using dashboard.Models;
using dashboard.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dashboard.Controllers
{
    [Authorize]
    [EnableCors]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public EmployeeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            EmployeeVM employeeVM = new EmployeeVM()
            {
                Employee = new Employee(),
                EmployeeSelectList = _db.departments.Select(i => new SelectListItem
                {
                    Text = i.Name.Trim(),
                    Value = i.Name.Trim(),
                }),
            };
            return View(employeeVM);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            EmployeeVM employeeVM = new EmployeeVM()
            {
                Employee = new Employee(),
                EmployeeSelectList = _db.departments.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString(),
                }),
            };
            if (id != null)
            {
                employeeVM.Employee = await _db.employees.Include(obj => obj.Department).FirstOrDefaultAsync(obj => obj.Id == id);
            }
            return View(employeeVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(EmployeeVM model)
        {
            if (ModelState.IsValid)
            {
                if (model.Employee.Id != 0)
                {
                    _db.employees.Update(model.Employee);
                }
                else
                {
                    _db.employees.Add(model.Employee);
                }
                _db.SaveChanges();
                return Redirect(nameof(Index));
            }
            model.EmployeeSelectList = _db.departments.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString(),
            });
            return View(model);
        }

        #region API Calls

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.employees.Include(c => c.Department).ToListAsync() });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            return Json(new { data = await _db.employees.Include(c => c.Department).ToListAsync() });
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetById(int id)
        {
            if (ModelState.IsValid)
            {
                return Json(new { data = await _db.employees.Include(obj => obj.Department).FirstOrDefaultAsync(obj => obj.Id == id) });
            }
            return Json(new { success = false, message = "Error while get data" });
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Update([FromBody] Employee model)
        {
            if (ModelState.IsValid)
            {
                var employee = _db.employees.FirstOrDefault(obj => obj.Id == model.Id);
                
                if(employee == null)
                {
                    return Json(new { success = false, message = "Error while updating" });
                }

                employee.Insurance = model.Insurance;
                employee.JobTitle = model.JobTitle;
                employee.EnglishName = model.EnglishName;
                employee.Email = model.Email;
                employee.Code = model.Code;
                employee.DepartmentId = model.DepartmentId;
                employee.Department = model.Department;
                employee.ArabicName = model.ArabicName;

                _db.SaveChanges();

                return Json(new { success = true, message = "update successfull" });
            }
            return Json(new { success = false, message = "Error while updating" });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Insert([FromBody] Employee model)
        {
            if (ModelState.IsValid && model.Id == 0)
            {
                _db.employees.Add(model);
                _db.SaveChanges();
                return Json(new { success = true, message = "insert successfull" });
            }
            return Json(new { success = false, message = "Error while adding" });
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.employees.FirstOrDefaultAsync(obj => obj.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _db.employees.Remove(user);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete successfull" });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var user = await _db.employees.FirstOrDefaultAsync(obj => obj.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _db.employees.Remove(user);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete successfull" });
        }

        #endregion
    }
}
