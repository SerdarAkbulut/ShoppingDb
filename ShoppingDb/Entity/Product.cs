

using ShoppingApi.Entity;
using System.ComponentModel.DataAnnotations;

namespace ShoppingDb.Entity
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }


        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public string? Discount { get; set; }

       
        public List<Image> Images { get; set; } = new List<Image>();
        
        public ICollection<ProductCategory> ProductCategories { get; set; }
       public ICollection<ProductVariant> ProductVariants { get; set; }


    }
}
