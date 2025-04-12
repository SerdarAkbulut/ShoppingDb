using ShoppingDb.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingApi.Entity
{
    public class Image
    {
        [Key]
        public int Id { get; set; }  

        public string? ImageUrl { get; set; }

        public string PublicId { get; set; }
       
        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}