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
                // Kullanıcı giriş başarılı, admin kontrolü yapalım
                if (user.IsAdmin)
                {
                    // Kullanıcı admin yetkisine sahip, admin rolünü ekleyin
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

                    return RedirectToAction("AdminDashboard", "Admin"); // Admin paneline yönlendirin
                }
                else
                {
                    // Diğer kullanıcılar için ana sayfaya yönlendirin
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, modelLogin.Email),
                new Claim(ClaimTypes.Role, "User") // Örnek olarak "User" rolünü ekleyin
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authenticationProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IsPersistent = modelLogin.KeepLoggedIn
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authenticationProperties);

                    return RedirectToAction("Index", "Home"); // Ana sayfaya yönlendirin
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
                // Şifre doğrulama işlemi
                if (model.Password != model.PasswordVerify)
                {
                    ModelState.AddModelError(string.Empty, "Password and confirmation password do not match.");
                    return View(model);
                }
                // Mail kontrol işlemi
                User existingUser = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "This email address is already registered.");
                    return View(model);
                }


                string salt = BCrypt.Net.BCrypt.GenerateSalt();
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password, salt, false, HashType.SHA256);

                var newUser = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = hashedPassword, // Bu şifreyi güvenli bir şekilde saklamalısınız.
                    BirthDate = model.BirthDate,
                    Address = model.Address,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    PhoneNumber = model.PhoneNumber,
                                               
                };
                _dbContext.Users.Add(newUser);
                _dbContext.SaveChanges();


                return RedirectToAction("Login");
            }

            return View(model);
        }


    }
}
