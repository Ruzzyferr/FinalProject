using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models.Classes
{
    public class Product
    {
        [Key] public int ProductId { get; set; }
        public string Name { get; set; }
        public String ProductImage { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
