using dashboard.Data;
using dashboard.Models;
using dashboard.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dashboard.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public EmployeeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
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
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.employees.Include(c => c.Department).ToListAsync() });
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            return Json(new { data = await _db.employees.Include(obj => obj.Department).FirstOrDefaultAsync(obj => obj.Id == id) });
        }

        [HttpPost]
        public IActionResult UpdateById(Employee model)
        {
            _db.employees.Update(model);
            _db.SaveChanges();
            return Json(new { success = true, message = "update successfull" });
        }

        [HttpPost]
        public IActionResult InsertById(Employee model)
        {
            _db.employees.Add(model);
            _db.SaveChanges();
            return Json(new { success = true, message = "insert successfull" });
        }



        [HttpDelete]
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

        #endregion
    }
}
