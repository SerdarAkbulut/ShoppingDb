using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.Entity
{
    public class Cargo
    {
        [Key]
        public int Id { get; set; }
        public decimal CargoFee { get; set; }

    }
}
