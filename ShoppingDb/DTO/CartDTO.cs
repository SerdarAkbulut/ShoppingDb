using ShoppingApi.Entity;
using ShoppingDb.Entity;

namespace ShoppingApi.DTO
{
    public class CartDTO
    {
        public int CartId { get; set; }
        public string userId { get; set; } = null!;

        public List<CartItemDTO> CartItems { get; set; } = new();
    }

    public class CartItemDTO
    {
        public int? ProductId { get;  set; }
        public String? Name { get; set; }
        public decimal? Price { get; set; }
    public String? ImageUrl { get; set; }    
        public int Quantity { get; set; }
    }
}
