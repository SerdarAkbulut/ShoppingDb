using ShoppingApi.Entity;

namespace ShoppingApi.DTO
{
    public class CreateAnonOrderDTO
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public AnonAddressDTO anonAddress { get; set; }


        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

        public List<OrderItemDTO> OrderItems { get; set; } = new();

        public CardDTO Card { get; set; }
    }

    public class AnonAddressDTO
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Sehir { get; set; }

        public string Ilce { get; set; }

        public string Cadde { get; set; }
        public string Sokak { get; set; }

        public string ApartmanNo { get; set; }
        public int DaireNo { get; set; }

        public string FullAddress { get; set; }
    }
 
}
