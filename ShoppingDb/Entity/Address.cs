using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApi.Entity
{
    public class Address
    {
        public int Id { get; set; }
        public string Country { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string Street { get; set; }

        public string ZipCode { get; set; }

        public string FullAddress { get; set; }

        public User User { get; set; }
        public string UserId { get; set; }
    }
}