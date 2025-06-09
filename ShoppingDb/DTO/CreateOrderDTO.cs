using ShoppingApi.Entity;
using System;
using System.Collections.Generic;

namespace ShoppingApi.DTO
{
    public class CreateOrderDTO
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public int AdressId { get; set; }


        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public List<OrderItemDTO> OrderItems { get; set; } = new();

        public CardDTO Card { get; set; }
    }

    public class CardDTO
    {
        
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string ExpireMonth { get; set; } 
        public string ExpireYear { get; set; }
        public string Cvc { get; set; }
        public int Installment { get; set; } = 1; 
    }

    public class OrderItemDTO
    {
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public decimal Price { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;

    }
}
