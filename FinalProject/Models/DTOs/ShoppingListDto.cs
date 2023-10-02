using FinalProject.Models.Classes;

namespace FinalProject.Models.DTOs
{
    public class ShoppingListDto
    {
        public int ShoppingListId { get; set; }
        public string Name { get; set; }
        public bool IsShoppingInProgress { get; set; }
        public bool IsShoppingCompleted { get; set; }
        public int UserId { get; set; }

        public List<Product> Products { get; set; }
        public List<ShoppingListItem> Items { get; set; }
    }
}
