using dashboard.Data;
using dashboard.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplicationIdentity.Models;

namespace dashboard.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext db, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _config = config;
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
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #region API Calls

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LoginWithJWT([FromBody] LoginVM login)
        {
            IActionResult response = Unauthorized();
            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
            {

                var token = GenerateJSONWebToken();
                response = Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), expiration = token.ValidTo });
            }

            return response;
        }

        private JwtSecurityToken GenerateJSONWebToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return token;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAll()
        {
            return Json(new
            {
                data = await _db.applicationUsers.Select(obj => new UpsertVM()
                {
                    Id = obj.Id,
                    Email = obj.Email,
                    EmployeeName = obj.EmployeeName,
                    Name = obj.UserName,
                    PhoneNumber = obj.PhoneNumber
                }).ToListAsync()
            });
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetById(string id)
        {
            if (ModelState.IsValid && id != null)
            {
                return Json(new
                {
                    data = await _db.applicationUsers.Select(obj => new UpsertVM()
                    {
                        Id = obj.Id,
                        Email = obj.Email,
                        EmployeeName = obj.EmployeeName,
                        Name = obj.UserName,
                        PhoneNumber = obj.PhoneNumber
                    }).FirstOrDefaultAsync(obj => obj.Id == id)
                });
            }
            return Json(new { success = false, message = "Error while get data" });
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Update([FromBody] UpsertVM model)
        {
            if (ModelState.IsValid && model.Id != "" && model.Id != null)
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
                    return Json(new { success = false, message = "No user with this id" });
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

                return Json(new { success = false, message = result.Errors.ElementAt(0).Description });
            }
            return Json(new { success = false, message = "Error while updating" });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Insert([FromBody] UpsertVM model)
        {
            if (ModelState.IsValid && (model.Id == "" || model.Id == null))
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
                return Json(new { success = false, message = result.Errors.ElementAt(0).Description });
            }

            return Json(new { success = false, message = "Error while adding", token = "" });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _db.applicationUsers.FirstOrDefaultAsync(obj => obj.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "No user with this id" });
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
