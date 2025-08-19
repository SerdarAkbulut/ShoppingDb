using ShoppingApi.Entity;

namespace ShoppingApi.DTO
{
    public class AddressDTO
    {
        public string AdSoyad { get; set; }
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