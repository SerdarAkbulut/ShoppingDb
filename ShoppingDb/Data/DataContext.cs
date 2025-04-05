using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.Entity;
using ShoppingDb.Entity;

namespace ShoppingApi.Data
{
    public class DataContext : IdentityDbContext<User, Role, string>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<Image> Images { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Önce İlişki Tanımla
            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Sonra HasData ile Seed Data Ekle
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Test Ürünü", Category = "Test Kategorisi", Description = "Açıklama", IsActive = true, Price = 15, Stock = 5 },
                new Product { Id = 2, Name = "Test Ürünü 2", Category = "Test Kategorisi 2", Description = "Açıklama 2", IsActive = true, Price = 20, Stock = 5 },
                new Product { Id = 3, Name = "Test Ürünü 3", Category = "Test Kategorisi 3", Description = "Açıklama 3", IsActive = true, Price = 18, Stock = 5 },
                new Product { Id = 4, Name = "Test Ürünü 4", Category = "Test Kategorisi 4", Description = "Açıklama 4", IsActive = true, Price = 9, Stock = 5 }
            );
        }
    }
}
