

using ShoppingApi.Entity;
using System.ComponentModel.DataAnnotations;

namespace ShoppingDb.Entity
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public string? Category { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsActive { get; set; }

       
        public List<Image> Images { get; set; } = new List<Image>();

        public int Stock { get; set; }
    }
}
