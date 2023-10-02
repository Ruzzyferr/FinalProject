using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FinalProject.Models.Classes;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using FinalProject.Models.DTOs;

namespace FinalProject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ProjectDbContext _context;


        public HomeController(ProjectDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.IsInRole("Admin")){
                return RedirectToAction("AdminDashboard", "Admin");
            }

            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value; // Şu anki kullanıcı kimliği
            var shoppingLists = await _context.ShoppingLists
    .Where(s => s.UserId == _context.Users.FirstOrDefault(u => u.Email == userEmail).UserId)
    .ToListAsync();

            // ShoppingListViewModel listesi oluşturun ve ShoppingList'leri dönüştürün
            var shoppingListViewModels = shoppingLists.Select(sl => new ShoppingListListViewModel
            {
                // ShoppingList'ten gerekli bilgileri alın ve ShoppingListViewModel'e atayın
                ShoppingListId = sl.ShoppingListId,
                Name = sl.Name,
                IsShoppingCompleted = sl.IsShoppingCompleted,
                IsShoppingInProgress = sl.IsShoppingInProgress,
            }).ToList();

            // shoppingListViewModels şimdi ShoppingListViewModel listesini içerir



            return View(shoppingListViewModels);
        }

        // Alışveriş listesi oluşturma işlevi
        [Authorize(Roles = "User")]
        public IActionResult CreateShoppingList()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateShoppingList([Bind("Name")] ShoppingListViewModel shoppingList)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcıyı bulun
                var userEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
                if (user == null)
                {
                    return RedirectToAction("Index"); // Kullanıcı bulunamazsa Index sayfasına yönlendirin
                }

                var shopinglist = new ShoppingList
                {
                    Name = shoppingList.Name,
                };

                shopinglist.UserId = user.UserId; // Kullanıcıyı alışveriş listesine ata
                _context.Add(shopinglist);
                _context.SaveChanges();

                // Alışveriş listesini oluşturduktan sonra Index sayfasına yönlendirirken modeli gönderin
                var shoppingLists = await _context.ShoppingLists
                    .Where(s => s.UserId == user.UserId)
                    .ToListAsync();

                var shoppingListViewModels = shoppingLists.Select(sl => new ShoppingListListViewModel
                {
                    // ShoppingList'ten gerekli bilgileri alın ve ShoppingListViewModel'e atayın
                    ShoppingListId = sl.ShoppingListId,
                    Name = sl.Name,
                    IsShoppingCompleted = sl.IsShoppingCompleted,
                    IsShoppingInProgress = sl.IsShoppingInProgress,
                }).ToList();

                return View("Index", shoppingListViewModels);
            }

            return View("Index");
        }


        // Alışveriş listesi görüntüleme işlevi
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ViewShoppingList(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoppingList =  _context.ShoppingLists
                .Include(s => s.Items)
                .FirstOrDefault(s => s.ShoppingListId == id && s.UserId == _context.Users.FirstOrDefault(u => u.Email == User.FindFirst(ClaimTypes.NameIdentifier).Value).UserId);

            if (shoppingList == null)
            {
                return NotFound();
            }

            

            ShoppingListDto shoppingListDto = new ShoppingListDto
            {
                ShoppingListId = shoppingList.ShoppingListId,
                Name = shoppingList.Name,
                Items = shoppingList.Items,
                
                UserId = shoppingList.UserId,
                IsShoppingCompleted = shoppingList.IsShoppingCompleted,
                IsShoppingInProgress= shoppingList.IsShoppingInProgress
            };

            return View(shoppingListDto);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteList(int Id)
        {
            var shoppingList =  _context.ShoppingLists.FirstOrDefault(s => s.ShoppingListId == Id);

            if (shoppingList != null)
            {
                _context.ShoppingLists.Remove(shoppingList);
                _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public IActionResult AddItem(int id)
        {
            var products = _context.Products.ToList(); // Tüm ürünleri alabilirsiniz, gerektiğinde filtrelemeyi yapabilirsiniz.

            List<VMProduct> vmProducts = new List<VMProduct>();

            foreach (var product in products)
            {
                VMProduct vmProduct = new VMProduct
                {
                    ID = product.ProductId,
                    ProductName = product.Name,
                    ProductImage = product.ProductImage
                };

                vmProducts.Add(vmProduct);
            }


            var shoppingListItem = new ShoppingListItemDto
            {
                ShoppingListId = id,
                products = vmProducts,
            };
            return View(shoppingListItem);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(ShoppingListItemDto shoppingListItemDto)
        {
            if (ModelState.IsValid)
            {
                // Seçilen ürünü ve açıklamayı kullanarak yeni bir ShoppingListItem oluşturun
                var shoppingListItem = new ShoppingListItem
                {
                    ShoppinglistId = shoppingListItemDto.ShoppingListId,
                    ProductName = shoppingListItemDto.ProductName,
                    ProductId = shoppingListItemDto.ProductId,
                    Product = _context.Products.FirstOrDefault(x => x.ProductId == shoppingListItemDto.ProductId),
                    Description = shoppingListItemDto.Description,
                    ProductImage = _context.Products.FirstOrDefault(x => x.ProductId == shoppingListItemDto.ProductId).ProductImage

                };

                // ShoppingListItem'i veritabanına ekleyin
                _context.ShoppingListItems.Add(shoppingListItem);
                await _context.SaveChangesAsync();

                // İlgili alışveriş listesine geri dönün
                return RedirectToAction("ViewShoppingList", new {
                id = shoppingListItemDto.ShoppingListId,
                });
            }

            var products = _context.Products.ToList(); // Tüm ürünleri alabilirsiniz, gerektiğinde filtrelemeyi yapabilirsiniz.

            List<VMProduct> vmProducts = new List<VMProduct>();

            foreach (var product in products)
            {
                VMProduct vmProduct = new VMProduct
                {
                    ID = product.ProductId,
                    ProductName = product.Name,
                    ProductImage = product.ProductImage
                };

                vmProducts.Add(vmProduct);
            }


            shoppingListItemDto.products = vmProducts;
            return View(shoppingListItemDto);
        }

        public IActionResult GoingToShopping(int id)
        {
            // Veritabanından ShoppingListId'ye göre ilgili alışveriş listesini alın
            var shoppingList = _context.ShoppingLists.FirstOrDefault(s => s.ShoppingListId == id);

            if (shoppingList == null)
            {
                // Belirtilen ShoppingListId'ye sahip alışveriş listesi bulunamazsa hata işleme kodunu ekleyin.
                return NotFound(); // Örnek bir hata durumu; isteğe bağlı olarak farklı bir işlem yapabilirsiniz.
            }

            // ShoppingListItemDto listesini oluşturun ve veritabanından ilgili ürünleri çekin
            var shoppingItems = _context.ShoppingListItems
                .Where(item => item.ShoppinglistId == id)
                .Select(item => new ShoppingListItemDto
                {
                    ShoppingListId = item.ShoppinglistId,
                    ProductName = item.Product.Name, // Ürün adını çekin
                    Description = item.Description,
                    // Diğer özellikleri doldurun
                })
                .ToList();

            

            return View(shoppingItems);
        }

        [HttpPost]
        public IActionResult RemoveFromList(int id)
        {
            // Veritabanından ilgili ürünü çekin
            var shoppingListItem = _context.ShoppingListItems.FirstOrDefault(item => item.ShoppingListItemId == id);

            if (shoppingListItem == null)
            {
                // Belirtilen ShoppingListItemId'ye sahip öğe bulunamazsa hata işleme kodunu ekleyin.
                return NotFound(); // Örnek bir hata durumu; isteğe bağlı olarak farklı bir işlem yapabilirsiniz.
            }

            // Ürünü veritabanından silin
            _context.ShoppingListItems.Remove(shoppingListItem);
            _context.SaveChanges();

            // Kullanıcıyı aynı alışveriş listesi sayfasına yönlendirin
            return RedirectToAction("ViewShoppingList", new { id = shoppingListItem.ShoppinglistId });
        }




















        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login","Access");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}