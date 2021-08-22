using dashboard.Data;
using dashboard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationIdentity.Models;

namespace dashboard.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string id)
        {
            UpsertVM registerViewModel = new UpsertVM();
            if (id != null)
            {
                var user = await _db.applicationUsers.FirstOrDefaultAsync(obj => obj.Id == id);

                registerViewModel.Id = user.Id;
                registerViewModel.Name = user.UserName;
                registerViewModel.Email = user.Email;
                registerViewModel.EmployeeName = user.EmployeeName;
                registerViewModel.UserSelectList = _db.employees.Select(i => new SelectListItem
                {
                    Text = i.EnglishName,
                    Value = i.EnglishName
                });
                registerViewModel.PhoneNumber = user.PhoneNumber;

            }
            return View(registerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(UpsertVM model)
        {
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser
                {
                    UserName = model.Name,
                    Email = model.Email,
                    EmployeeName = model.EmployeeName,
                    PhoneNumber = model.PhoneNumber,
                };

                IdentityResult result;

                if (model.Id != null && model.Id != "")
                {
                    var oldUser = await _userManager.FindByIdAsync(model.Id) as ApplicationUser;
                    if (oldUser == null)
                    {
                        ModelState.AddModelError(string.Empty, "This user doe not exist");
                        RedirectToAction(nameof(Index));
                    }

                    var code = await _userManager.GeneratePasswordResetTokenAsync(oldUser);
                    result = await _userManager.ResetPasswordAsync(oldUser, code, model.Password);

                    if (!result.Succeeded)
                    {
                        AddErrors(result);
                    }
                    oldUser.Email = model.Email;
                    oldUser.UserName = model.Name;
                    oldUser.PhoneNumber = model.PhoneNumber;
                    oldUser.EmployeeName = model.EmployeeName;
                    result = await _userManager.UpdateAsync(oldUser);
                }
                else
                {
                    result = await _userManager.CreateAsync(user, model.Password);
                }

                if (result.Succeeded)
                {
                    return Redirect(nameof(Index));
                }
                AddErrors(result);
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult Login(string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            returnurl = returnurl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnurl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.applicationUsers.ToListAsync() });
        }

        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            return Json(new { data = await _db.applicationUsers.FirstOrDefaultAsync(obj => obj.Id == id) });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateById([FromBody] UpsertVM model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Name,
                Email = model.Email,
                EmployeeName = model.EmployeeName,
                PhoneNumber = model.PhoneNumber,
            };

            var oldUser = await _userManager.FindByIdAsync(model.Id) as ApplicationUser;
            if (oldUser == null)
            {
                return Json(new { success = false, message = "Error while updating" });
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(oldUser);
            var result = await _userManager.ResetPasswordAsync(oldUser, code, model.Password);

            if (!result.Succeeded)
            {
                return Json(new { success = false, message = "Error while updating" });
            }
            oldUser.Email = model.Email;
            oldUser.UserName = model.Name;
            oldUser.PhoneNumber = model.PhoneNumber;
            oldUser.EmployeeName = model.EmployeeName;
            result = await _userManager.UpdateAsync(oldUser);

            if (result.Succeeded)
            {
                return Json(new { success = true, message = "update successfull" });
            }
            return Json(new { success = false, message = "Error while updating" });
        }

        [HttpPost]
        public async Task<IActionResult> InsertById([FromBody] UpsertVM model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Name,
                Email = model.Email,
                EmployeeName = model.EmployeeName,
                PhoneNumber = model.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Json(new { success = true, message = "insert successfull" });
            }
            return Json(new { success = false, message = "Error while adding" });
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _db.applicationUsers.FirstOrDefaultAsync(obj => obj.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Delete successfull" });
            }
            return Json(new { success = false, message = "Error while deleting" });
        }
        #endregion
    }
}
