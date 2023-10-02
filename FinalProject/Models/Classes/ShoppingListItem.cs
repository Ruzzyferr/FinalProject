using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models.Classes
{
    public class ShoppingListItem
    {

        [Key] public int ShoppingListItemId { get; set; }
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductImage { get; set; }
        public int ShoppinglistId{ get; set; }

        public Product Product { get; set; }
        public String Description { get; set; }
    }
}
