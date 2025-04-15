using ShoppingDb.Entity;

namespace ShoppingApi.Entity
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public decimal SubTotal { get; set; }

        public decimal DeliveryFee { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }

        public decimal GetTotal()
        {
            return SubTotal + DeliveryFee;
        }

    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public Order Order { get; set; }
        public int OrderId { get; set; }

        public decimal Price { get; set; }
        public Product Product { get; set; } = null!;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductImages { get; set; }

    }
    public enum OrderStatus
    {
        Pending,
        Approved,
        PaymentFailed,
        Completed

    }
}
