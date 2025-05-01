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
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Size> Sizes { get; set; }
    public DbSet<ProductVariant> ProductVariants { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Önce İlişki Tanımla
            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
          
  
            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId }); 
        
            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId);

            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(pv => pv.ProductId);

            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Color)
                .WithMany()
                .HasForeignKey(pv => pv.ColorId);

            modelBuilder.Entity<ProductVariant>()
                .HasOne(pv => pv.Size)
                .WithMany()
                .HasForeignKey(pv => pv.SizeId);

           
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Test Ürünü",  Description = "Açıklama", IsActive = true, Price = 15,  },
                new Product { Id = 2, Name = "Test Ürünü 2",  Description = "Açıklama 2", IsActive = true, Price = 20, },
                new Product { Id = 3, Name = "Test Ürünü 3",  Description = "Açıklama 3", IsActive = true, Price = 18,  },
                new Product { Id = 4, Name = "Test Ürünü 4",  Description = "Açıklama 4", IsActive = true, Price = 9, }
            );
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Pantalon",  },
                new Category { Id = 2, Name = "Etek", },
                new Category { Id = 3, Name = "Mont", },
                new Category { Id = 4, Name = "Kap",  },
                new Category { Id = 5, Name = "Tunik",},
                new Category { Id = 6, Name = "Elbise",  },
                new Category { Id = 7, Name = "Takım",},
                new Category { Id = 8, Name = "Triko Hırka",  },
                new Category { Id = 9, Name = "Badi Üst", }
            );

            modelBuilder.Entity<ProductCategory>().HasData(
                new ProductCategory { ProductId = 1, CategoryId = 1 },
                new ProductCategory { ProductId = 1, CategoryId = 2 },
                new ProductCategory { ProductId = 1, CategoryId = 3 },

                new ProductCategory { ProductId = 2, CategoryId = 2 },
                new ProductCategory { ProductId = 3, CategoryId = 3 },
                new ProductCategory { ProductId = 4, CategoryId = 4 }
            );
            modelBuilder.Entity<Color>().HasData(
                new Color { Id = 1, Name = "Kırmızı" },
                new Color { Id = 2, Name = "Mavi" },
                new Color { Id = 3, Name = "Siyah" },
                new Color { Id = 4, Name = "Beyaz" },
                new Color { Id = 5, Name = "Yeşil" },
                new Color { Id = 6, Name = "Bej" },
                new Color { Id = 7, Name = "Bordo" },
                new Color { Id = 8, Name = "Lacivert" },
                new Color { Id = 9, Name = "Zümrüt Yeşili" },
                new Color { Id = 10, Name = "Pudra Pembe" },
                new Color { Id = 11, Name = "Mint Yeşili" },
                new Color { Id = 12, Name = "Gri" }
            );
                modelBuilder.Entity<Size>().HasData(
                    new Size { Id = 1, Name = "36" },
                    new Size { Id = 2, Name = "38" },
                    new Size { Id = 3, Name = "40" },
                    new Size { Id = 4, Name = "42" },
                    new Size { Id = 5, Name = "44" },
                    new Size { Id = 6, Name = "46" },
                    new Size { Id = 7, Name = "48" },
                    new Size { Id = 8, Name = "50" },
                    new Size { Id = 9, Name = "52" },
                    new Size { Id = 10, Name = "54" },
                    new Size { Id = 11, Name = "56" }
                );

            modelBuilder.Entity<ProductVariant>().HasData(
                new ProductVariant { Id = 1, ProductId = 1, ColorId = 3, SizeId = 2, Stock = 8 },  // Siyah - M
                new ProductVariant { Id = 2, ProductId = 1, ColorId = 3, SizeId = 3, Stock = 12 }, // Siyah - L
                new ProductVariant { Id = 3, ProductId = 1, ColorId = 4, SizeId = 2, Stock = 5 }   // Beyaz - M
            );
        }
    }
}
