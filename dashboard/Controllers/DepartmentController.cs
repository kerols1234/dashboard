using dashboard.Data;
using dashboard.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dashboard.Controllers
{
    [Authorize]
    [EnableCors]
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _db;
        public DepartmentController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int? id)
        {
            Department department = new Department() { Name = "" };
            if (id != null)
            {
                department = _db.departments.FirstOrDefault(obj => obj.Id == id);
            }

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Department model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    await _db.departments.AddAsync(model);
                }
                else
                {
                    Department department = await _db.departments.FirstOrDefaultAsync(obj => obj.Id == model.Id);
                    department.Name = model.Name;
                }

                _db.SaveChanges();
            }

            model = new Department() { Name = "" };
            return View(model);
        }

        #region API Calls

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.departments.ToListAsync() });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            return Json(new { data = await _db.departments.ToListAsync() });
        }


        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetById(int id)
        {
            if(ModelState.IsValid)
            {
                return Json(new { data = await _db.departments.FirstOrDefaultAsync(obj => obj.Id == id) });
            }
            return Json(new { success = false, message = "Error while get data" });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Update([FromBody] Department model)
        {
            if(ModelState.IsValid)
            {
                Department department = await _db.departments.FirstOrDefaultAsync(obj => obj.Id == model.Id);
                if(department == null)
                {
                    return Json(new { success = false, message = "Error while updating" });
                }
                department.Name = model.Name;

                _db.SaveChanges();
                return Json(new { success = true, message = "update successfull" });
            }
            return Json(new { success = false, message = "Error while updating" });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Insert([FromBody] Department model)
        {
            if(ModelState.IsValid && model.Id == 0)
            {
                var obj = await _db.departments.AddAsync(model);
                _db.SaveChanges();
                return Json(new { success = true, message = "insert successfull" });
            }
            return Json(new { success = false, message = "Error while adding" });
        }

        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _db.departments.FirstOrDefaultAsync(obj => obj.Id == id);
            if (department == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _db.departments.Remove(department);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete successfull" });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _db.departments.FirstOrDefaultAsync(obj => obj.Id == id);
            if (department == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _db.departments.Remove(department);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete successfull" });
        }

        #endregion
    }
}
