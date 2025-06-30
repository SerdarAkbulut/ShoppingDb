using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.Entity
{
    public class Slider
    {
        [Key]
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public string PublicId { get; set; }
       
    }
}
