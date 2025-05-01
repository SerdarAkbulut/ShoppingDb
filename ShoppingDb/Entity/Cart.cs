using ShoppingDb.Entity;
using System.Text.Json.Serialization;

namespace ShoppingApi.Entity
{
    public class Cart
    {
        public int CartId { get; set; }
        public string userId { get; set; } = null!;
     
        public List<CartItem> CartItems { get; set; } = new();

        public void AddItem(Product product, int quantity)
        {
            var item = CartItems.Where(c => c.ProductId == product.Id).FirstOrDefault();

            if (item == null)
            {
                CartItems.Add(new CartItem { Product = product, Quantity = quantity });
            }
            else
            {
                item.Quantity += quantity;
            }
        }

    public void DeleteItem(int productId, int quantity)
{
    var item = CartItems.Where(c => c.ProductId == productId).FirstOrDefault();

    if (item == null) return;

    item.Quantity -= quantity;

    if (item.Quantity <= 0)
    {
        CartItems.Remove(item);
    }
}
    }
    public class CartItem
    {
        public int CartItemId { get; private set; }
        public int ProductId { get; private set; }
        public Product Product { get; set; }
        public int CartId { get; set; }
        public Cart Cart {  get; set; }

        public int Quantity { get; set; }
    }
}
