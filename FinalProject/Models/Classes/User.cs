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
        public string PhoneNumber { get; set; } // Telefon numarası
        public DateTime BirthDate { get; set; } // Doğum tarihi
        public string Address { get; set; } // Adres bilgisi
        public string City { get; set; } // Şehir
        public string PostalCode { get; set; } // Posta kodu
        public bool IsAdmin { get; set; } // Yönetici veya kullanıcı rolü
        public List<ShoppingList> ShoppingLists { get; set; } // Kullanıcının oluşturduğu alışveriş listeleri
    }
}
