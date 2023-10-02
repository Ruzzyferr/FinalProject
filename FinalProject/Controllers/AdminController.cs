using FinalProject.Models;
using FinalProject.Models.Classes;
using FinalProject.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class AdminController : Controller
    {

        private readonly ProjectDbContext _dbContext;

        public AdminController(ProjectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddCategory() { return View(); }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddCategory(VMCategory model)
        {
            
            var existingCategory = _dbContext.Categories.FirstOrDefault(c => c.Name == model.CategoryName);

            if (existingCategory != null)
            {
                
                ViewData["CategoryExists"] = "Bu kategori zaten mevcut.";
                return View(model);
            }

            
            var newCategory = new Category
            {
                Name = model.CategoryName
            };

            _dbContext.Categories.Add(newCategory);
            _dbContext.SaveChanges();

            
            return RedirectToAction("EditCategory"); 
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditCategory() {

            var categories = _dbContext.Categories.ToList();
            var viewModel = new EditCategoryViewModel
            {
                Categories = categories
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditCategoryy(int id)
        {
            var category = _dbContext.Categories.Find(id);

            if (category == null)
            {
                
                return NotFound();
            }

            return View(category);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult UpdateCategory(Category model)
        {
            
            var existingCategory = _dbContext.Categories.FirstOrDefault(c => c.Name == model.Name || c.CategoryId != model.CategoryId);

            if (existingCategory != null)
            {
                
                ViewData["CategoryExists"] = "Bu kategori zaten mevcut.";
                return View("EditCategory", model);
            }

            
            var categoryToUpdate = _dbContext.Categories.Find(model.CategoryId);
            if (categoryToUpdate != null)
            {
                categoryToUpdate.Name = model.Name;
                _dbContext.Categories.Update(categoryToUpdate);
                _dbContext.SaveChanges();
            }

            
            return RedirectToAction("EditCategory");
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteCategory(int id) {

            var categoryToDelete = _dbContext.Categories.Find(id);
            if (categoryToDelete != null)
            {
                _dbContext.Categories.Remove(categoryToDelete);
                _dbContext.SaveChanges();
            }

            
            return RedirectToAction("EditCategory");

            }




        [Authorize(Roles = "Admin")]
        public IActionResult ListProducts() {

            var categories = _dbContext.Categories.ToList();
            var viewModel = new ProductByCategoryViewModel
            {
                Categories = categories
            };

            return View(viewModel);

            }

        
        [Authorize(Roles = "Admin")]
        public IActionResult ProductByCategory(int SelectedCategoryId)
        {
           
            var productsInCategory = _dbContext.Products.Where(p => p.CategoryId == SelectedCategoryId).ToList();

            
            var viewModel = new ProductByCategoryViewModel
            {
                SelectedCategoryId = SelectedCategoryId,
                Products = productsInCategory
            };

            return View(viewModel);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult SaveProduct(int SelectedCategoryId)
        {
            var viewModel = new VMProduct
            {
                CategoryId = SelectedCategoryId,
            };
            return View(viewModel);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult SaveProductt(VMProduct model)
        {
            if (ModelState.IsValid)
            {

                var existingProduct = _dbContext.Products.FirstOrDefault(c => c.Name == model.ProductName );

                if (existingProduct != null)
                {
                    ViewData["ProductExists"] = "Bu Ürün zaten mevcut.";
                    return RedirectToAction("SaveProduct", existingProduct.CategoryId);
                }

                
                Product newProduct = new Product
                {
                    Name = model.ProductName,
                    ProductImage = model.ProductImage, 
                    CategoryId = model.CategoryId,
                    Category = _dbContext.Categories.Find(model.CategoryId)
                };

                
                _dbContext.Products.Add(newProduct);
                _dbContext.SaveChanges();

                
                return RedirectToAction("ListProducts");
            }

            
            return View("SaveProduct", model);
        }



        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult EditProduct(int id) {

            
            var product = _dbContext.Products.FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound(); 
            }

            VMProduct product1 = new VMProduct
            {
                ID = product.ProductId,
                ProductName = product.Name,
                CategoryId = product.CategoryId,
                ProductImage = product.ProductImage,
            };
            
            

            
            return View(product1);

            }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult EditProduct(VMProduct editedProduct)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.ProductId == editedProduct.ID);

            product.Name = editedProduct.ProductName;

            if (ModelState.IsValid)
            {
                
                _dbContext.Products.Update(product);
                _dbContext.SaveChanges();

                
                return RedirectToAction("ProductByCategory", new { SelectedCategoryId = editedProduct.CategoryId });
            }
            else
            {
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                    }
                }
            }
            

            
            return View(editedProduct);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult DeleteProduct(int id) {

            var product = _dbContext.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null) { return NotFound();}
            int categoryid = product.CategoryId;
            if (ModelState.IsValid)
            {
                
                _dbContext.Products.Remove(product);
                _dbContext.SaveChanges();

                
                return RedirectToAction("ProductByCategory", new { SelectedCategoryId = categoryid });
            }

           

            return View(); }
    }
}
