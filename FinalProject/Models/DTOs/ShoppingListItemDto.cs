using FinalProject.Models.Classes;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;

namespace FinalProject.Models.DTOs
{
    public class ShoppingListItemDto
    {

        public int ShoppingListItemId { get; set; }
        public int ShoppingListId { get; set; }

        [ValidateNever]
        public List<VMCategory> Categories { get; set; }

        [ValidateNever]
        public string ProductName { get; set; }

        public int ProductId { get; set; }
        public List<VMProduct> products { get; set; }

        [DefaultValue(false)]
        public bool IsChecked { get; set; }

        public String Description { get; set; }
    }
}
