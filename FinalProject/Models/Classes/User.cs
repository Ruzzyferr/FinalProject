using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models.Classes
{
    public class User
    {

        [Key] public int UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; } // Yönetici veya kullanıcı rolü
        public List<ShoppingList> ShoppingLists { get; set; } // Kullanıcının oluşturduğu alışveriş listeleri
    }
}
