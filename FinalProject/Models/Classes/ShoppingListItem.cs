using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models.Classes
{
    public class ShoppingListItem
    {

        [Key] public int ShoppingListItemId { get; set; }
        public int ShoppingListId { get; set; }
        public ShoppingList ShoppingList { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
