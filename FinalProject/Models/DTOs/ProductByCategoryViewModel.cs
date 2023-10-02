using FinalProject.Models.Classes;

namespace FinalProject.Models.DTOs
{
    public class ProductByCategoryViewModel
    {
        public List<Category> Categories { get; set; } = new List<Category>();
        public int SelectedCategoryId { get; set; }
        public List<Product> Products { get; set; }
    }
}
