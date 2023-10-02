using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models.Classes
{
    public class ShoppingList
    {

        [Key] public int ShoppingListId { get; set; }
        public string Name { get; set; }
        public bool IsShoppingInProgress { get; set; }
        public bool IsShoppingCompleted { get; set; }

        public List<ShoppingListItem> Items { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }

}
