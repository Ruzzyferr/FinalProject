using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinalProject.Models.Classes;
using Microsoft.CodeAnalysis.Scripting;
using FinalProject.Models.DTOs;

namespace FinalProject.Controllers
{
    public class AccessController : Controller
    {

        private readonly ProjectDbContext _dbContext;

        public AccessController(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Login()
        {

            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated && claimUser.IsInRole("User")) { return RedirectToAction("Index", "Home"); }
            if (claimUser.Identity.IsAuthenticated && claimUser.IsInRole("Admin")) { return RedirectToAction("AdminDashboard", "Admin"); }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(VMLogin modelLogin)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == modelLogin.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(modelLogin.Password, user.Password))
            {
                
                if (user.IsAdmin)
                {
                    
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, modelLogin.Email),
                new Claim(ClaimTypes.Role, "Admin")
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authenticationProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IsPersistent = modelLogin.KeepLoggedIn
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authenticationProperties);

                    return RedirectToAction("AdminDashboard", "Admin"); 
                }
                else
                {
                    
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, modelLogin.Email),
                new Claim(ClaimTypes.Role, "User") 
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authenticationProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IsPersistent = modelLogin.KeepLoggedIn
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authenticationProperties);

                    return RedirectToAction("Index", "Home"); 
                }
            }

            ViewData["ValidateMessage"] = "User not found or invalid password.";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(VMRegister model)
        {
            if (ModelState.IsValid)
            {
                
                if (model.Password != model.PasswordVerify)
                {
                    ViewData["ValidateMessage"] = "Şifreler Birbiri ile eşleşmiyor";
                    return View(model);
                }

                if (model.Password.Length < 8)
                {
                    ViewData["ValidateMessage"] = "Şifre en az 8 karakter içermelidir";
                    return View(model);
                }

                if (!IsPasswordComplex(model.Password))
                {
                    ViewData["ValidateMessage"] = "Şifre en az bir büyük harf, bir küçük harf ve bir rakam içermelidir";
                    return View(model);
                }

                User existingUser = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ViewData["ValidateMessage"] = "Bu Mail adresi zaten kayıtlı";
                    return View(model);
                }


                string salt = BCrypt.Net.BCrypt.GenerateSalt();
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password, salt, false, HashType.SHA256);

                var newUser = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = hashedPassword, 
                                               
                };
                _dbContext.Users.Add(newUser);
                _dbContext.SaveChanges();


                return RedirectToAction("Login");
            }

            return View(model);
        }

        private bool IsPasswordComplex(string password)
        {
            bool hasUpperCase = false;
            bool hasLowerCase = false;
            bool hasDigit = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c))
                {
                    hasUpperCase = true;
                }
                else if (char.IsLower(c))
                {
                    hasLowerCase = true;
                }
                else if (char.IsDigit(c))
                {
                    hasDigit = true;
                }

                if (hasUpperCase && hasLowerCase && hasDigit)
                {
                    return true;
                }
            }

            return false;
        }


    }
}
