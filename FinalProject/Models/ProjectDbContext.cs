using FinalProject.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Models
{
    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext()
        {
        }

        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<ShoppingListItem> ShoppingListItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Veritabanı tablolarını ve ilişkilerini tanımlamak için gerekli kodları buraya ekleyebilirsiniz.

            // Örnek: modelBuilder.Entity<ShoppingList>().HasMany(sl => sl.Items).WithOne(sli => sli.ShoppingList);
        }

    }
}
