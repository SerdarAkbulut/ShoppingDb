using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ShoppingApi.Entity;

namespace ShoppingDb.Entity
{
    public class ProductVariant
    {
         [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int ColorId { get; set; }
    public Color Color { get; set; }

    public int SizeId { get; set; }
    public Size Size { get; set; }

    public int Stock { get; set; }
    }
}