using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApi.Entity
{
    public class Address
    {
        [Key]
        public int Id { get; set; }
        public string AdSoyad { get; set; }
        public string Phone { get; set; }
        public string Sehir { get; set; }

        public string Ilce { get; set; }

        public string Cadde { get; set; }
        public string Sokak { get; set; }

        public int ApartmanNo { get; set; }
        public int DaireNo { get; set; }

        public string FullAddress { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        public string UserId { get; set; }
    }
}