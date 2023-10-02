using FinalProject.Models.Classes;

namespace FinalProject.Models.DTOs
{
    public class ShoppingListListViewModel
    {

        public int ShoppingListId { get; set; }
        public string Name { get; set; }
        public bool IsShoppingInProgress { get; set; }
        public bool IsShoppingCompleted { get; set; }
        
    }
}
