using ShoppingApi.Entity;
using ShoppingDb.Entity;

namespace ShoppingApi.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public List<OrderItemDTO> OrderItems { get; set; } = new();

        public decimal SubTotal { get; set; }

        public decimal DeliveryFee { get; set; }
        public string UserId { get; set; }


    }

    public class OrderItemDTO
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }

        public decimal Price { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;

        public string ProductImage { get; set; } = null!;
    }
}
