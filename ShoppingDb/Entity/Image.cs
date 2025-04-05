using ShoppingDb.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingApi.Entity
{
    public class Image
    {
        [Key]
        public int Id { get; set; }  // ✅ Primary Key Ekledik

        [Required]
        public string? ImageUrl { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}