using ShoppingApi.Entity;
using ShoppingDb.Entity;

namespace ShoppingApi.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string Address { get; set; }

        public string Phone { get; set; }

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public List<OrderItemDTO> OrderItems { get; set; } = new();

        public string SubTotal { get; set; }

        public decimal DeliveryFee { get; set; }
        public string UserId { get; set; }


    }

 
}
