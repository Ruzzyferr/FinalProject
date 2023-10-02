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

        [Authorize(Roles = "User")]
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

            
            var shoppingListViewModels = shoppingLists.Select(sl => new ShoppingListListViewModel
            {
                
                ShoppingListId = sl.ShoppingListId,
                Name = sl.Name,
                IsShoppingCompleted = sl.IsShoppingCompleted,
                IsShoppingInProgress = sl.IsShoppingInProgress,
            }).ToList();

            



            return View(shoppingListViewModels);
        }

        
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
                
                var userEmail = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = _context.Users.FirstOrDefault(u => u.Email == userEmail);
                if (user == null)
                {
                    return RedirectToAction("Index"); 
                }

                var shopinglist = new ShoppingList
                {
                    Name = shoppingList.Name,
                };

                shopinglist.UserId = user.UserId; 
                _context.Add(shopinglist);
                _context.SaveChanges();

                
                var shoppingLists = await _context.ShoppingLists
                    .Where(s => s.UserId == user.UserId)
                    .ToListAsync();

                var shoppingListViewModels = shoppingLists.Select(sl => new ShoppingListListViewModel
                {
                    
                    ShoppingListId = sl.ShoppingListId,
                    Name = sl.Name,
                    IsShoppingCompleted = sl.IsShoppingCompleted,
                    IsShoppingInProgress = sl.IsShoppingInProgress,
                }).ToList();

                return View("Index", shoppingListViewModels);
            }

            return View("Index");
        }


        
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
        
        [Authorize(Roles = "User")]
        public IActionResult AddItem(int id, int categoryId = 0)
        {
            var products = _context.Products.ToList();
            var categories = _context.Categories.ToList();

            List<VMProduct> vmProducts = new List<VMProduct>();
            List<VMCategory> vmCategories = new List<VMCategory>();

            foreach (var product in products)
            {
                VMProduct vmProduct = new VMProduct
                {
                    ID = product.ProductId,
                    ProductName = product.Name,
                    ProductImage = product.ProductImage,
                    CategoryId = product.CategoryId,
                };

                vmProducts.Add(vmProduct);
            }

            foreach (var category in categories)
            {
                VMCategory vMCategory = new VMCategory
                {
                    Id = category.CategoryId,
                    CategoryName = category.Name
                };
                vmCategories.Add(vMCategory);
            }


            if (categoryId == 0)
            {
                var shoppingListItem = new ShoppingListItemDto
                {
                    ShoppingListId = id,
                    products = vmProducts,
                    Categories = vmCategories
                };
                return View(shoppingListItem);
            }
            else 
            {
                var filteredProducts = vmProducts.Where(p => p.CategoryId == categoryId).ToList();
                var shoppingListItem = new ShoppingListItemDto
                {
                    ShoppingListId = id,
                    products = filteredProducts,
                    Categories = vmCategories
                };
                return View(shoppingListItem);
            }
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> AddItem(ShoppingListItemDto shoppingListItemDto)
        {
            if (ModelState.IsValid)
            {
                
                var shoppingListItem = new ShoppingListItem
                {
                    ShoppinglistId = shoppingListItemDto.ShoppingListId,
                    ProductName = shoppingListItemDto.ProductName,
                    ProductId = shoppingListItemDto.ProductId,
                    Product = _context.Products.FirstOrDefault(x => x.ProductId == shoppingListItemDto.ProductId),
                    Description = shoppingListItemDto.Description,
                    ProductImage = _context.Products.FirstOrDefault(x => x.ProductId == shoppingListItemDto.ProductId).ProductImage,
                    
                };

                
                _context.ShoppingListItems.Add(shoppingListItem);
                await _context.SaveChangesAsync();

                
                return RedirectToAction("ViewShoppingList", new {
                id = shoppingListItemDto.ShoppingListId,
                });
            }

            var products = _context.Products.ToList(); 

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

        [Authorize(Roles = "User")]
        public IActionResult GoingToShopping(int id)
        {
            
            var shoppingList = _context.ShoppingLists.FirstOrDefault(s => s.ShoppingListId == id);

            if (shoppingList == null)
            {
                
                return NotFound(); 
            }

            
            var shoppingItems = _context.ShoppingListItems
                .Where(item => item.ShoppinglistId == id)
                .Select(item => new ShoppingListItemDto
                {
                    ShoppingListItemId = item.ShoppingListItemId,
                    ShoppingListId = item.ShoppinglistId,
                    ProductName = item.Product.Name, 
                    Description = item.Description,
                    
                })
                .ToList();

            

            return View(shoppingItems);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult CompleteShopping(List<ShoppingListItemDto> shoppingListItems)
        {
            if (shoppingListItems != null && shoppingListItems.Any())
            {
                
                var itemsToRemove = shoppingListItems.Where(item => item.IsChecked).ToList();

                if (itemsToRemove.Any())
                {
                    foreach (var itemToRemove in itemsToRemove)
                    {
                        
                        var item = _context.ShoppingListItems.FirstOrDefault(i => i.ShoppingListItemId == itemToRemove.ShoppingListItemId);
                        if (item != null)
                        {
                            
                            _context.ShoppingListItems.Remove(item);
                        }
                    }

                    
                    _context.SaveChanges();
                }
            }

            
            return RedirectToAction("Index");
        }



        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult RemoveFromList(int id)
        {
            
            var shoppingListItem = _context.ShoppingListItems.FirstOrDefault(item => item.ShoppingListItemId == id);

            if (shoppingListItem == null)
            {
                
                return NotFound(); 
            }

            
            _context.ShoppingListItems.Remove(shoppingListItem);
            _context.SaveChanges();

            
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