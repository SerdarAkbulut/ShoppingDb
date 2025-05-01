using ShoppingDb.Entity;

namespace ShoppingApi.Entity
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string Sehir { get; set; }

        public string Ilce { get; set; }

        public string Cadde { get; set; }
        public string Sokak { get; set; }

        public int ApartmanNo { get; set; }
        public int DaireNo { get; set; }

        public string FullAddress { get; set; }

        public string Phone { get; set; }

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public decimal SubTotal { get; set; }

        public User User { get; set; }
        public string UserId { get; set; }


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
        public string Name { get; set; } = null!;

    }
    public enum OrderStatus
    {
        Pending,
        Approved,
        PaymentFailed,
        Completed

    }
}
