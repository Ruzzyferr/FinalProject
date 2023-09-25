using FinalProject.Models;
using FinalProject.Models.Classes;
using FinalProject.Models.DTOs;
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


        public IActionResult AdminDashboard()
        {
            return View();
        }

        public IActionResult AddCategory() { return View(); }

        [HttpPost]
        public IActionResult AddCategory(VMCategory model)
        {
            // Kategori adının benzersiz olup olmadığını kontrol edelim
            var existingCategory = _dbContext.Categories.FirstOrDefault(c => c.Name == model.CategoryName);

            if (existingCategory != null)
            {
                // Aynı isimde bir kategori zaten var, kullanıcıya uyarı verelim
                ViewData["CategoryExists"] = "Bu kategori zaten mevcut.";
                return View(model);
            }

            // Aynı isimde kategori yoksa, yeni kategoriyi veritabanına ekleyelim
            var newCategory = new Category
            {
                Name = model.CategoryName
            };

            _dbContext.Categories.Add(newCategory);
            _dbContext.SaveChanges();

            // Kategori başarıyla eklendi, kullanıcıyı isteğe bağlı olarak başka bir sayfaya yönlendirebilirsiniz
            return RedirectToAction("EditCategory"); // Örnek olarak kategorilerin listelendiği sayfaya yönlendiriyoruz
        }


        public IActionResult EditCategory() {

            var categories = _dbContext.Categories.ToList();

            // Kategori listesini ViewBag veya ViewData ile görünüme iletiyoruz
            ViewBag.Categories = categories;

            return View(); }

        public IActionResult EditCategoryy(int id)
        {
            var category = _dbContext.Categories.Find(id);

            if (category == null)
            {
                // Kategori bulunamadı, hata sayfasına yönlendirilebilir
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult UpdateCategory(Category model)
        {
            // Kategori adının benzersiz olup olmadığını kontrol edelim
            var existingCategory = _dbContext.Categories.FirstOrDefault(c => c.Name == model.Name && c.CategoryId != model.CategoryId);

            if (existingCategory != null)
            {
                // Aynı isimde bir kategori zaten var, kullanıcıya uyarı verelim
                ViewData["CategoryExists"] = "Bu kategori zaten mevcut.";
                return View("EditCategory", model);
            }

            // Kategori adını güncelle
            var categoryToUpdate = _dbContext.Categories.Find(model.CategoryId);
            if (categoryToUpdate != null)
            {
                categoryToUpdate.Name = model.Name;
                _dbContext.Categories.Update(categoryToUpdate);
                _dbContext.SaveChanges();
            }

            // Kategori başarıyla güncellendi, isteğe bağlı olarak başka bir sayfaya yönlendirebilirsiniz
            return RedirectToAction("EditCategory");
        }



        public IActionResult DeleteCategory() { return View(); }

        public IActionResult AddProduct() { return View(); }

        public IActionResult EditProduct() { return View(); }

        public ActionResult DeleteProduct() { return View(); }
    }
}
