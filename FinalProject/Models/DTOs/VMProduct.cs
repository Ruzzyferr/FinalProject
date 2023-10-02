using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models.DTOs
{
    public class VMProduct
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Ürün adı zorunludur.")]
        [Display(Name = "Ürün Adı")]
        public string ProductName { get; set; }

        [Display(Name = "Ürün Görseli")]
        public String ProductImage { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

    }
}
