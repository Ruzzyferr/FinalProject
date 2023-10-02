using FinalProject.Models.Classes;

namespace FinalProject.Models.DTOs
{
    public class ProductsByCategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<Product> Products { get; set; }
    }
}
