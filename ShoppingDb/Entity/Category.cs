using ShoppingDb.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ShoppingApi.Entity{
    
    public class Category{
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
  

        public ICollection<ProductCategory> ProductCategories { get; set; }
    }
}