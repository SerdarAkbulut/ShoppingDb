using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingApi.DTO;
using ShoppingApi.Entity;

namespace ShoppingApi.extensions
{
    public static class OrderExtensions
    {
        public static IQueryable<OrderDTO> OrderToDTO(this IQueryable<Order> query) 
        {
            return query.Select(x => new OrderDTO
            {
                OrderDate = x.OrderDate,
                Phone = x.Phone,
                UserId = x.UserId,
                SubTotal = x.SubTotal.ToString("N2", new System.Globalization.CultureInfo("tr-TR")),
                OrderStatus = x.OrderStatus,
                OrderItems = x.OrderItems.Select(item => new OrderItemDTO
                {
                    OrderId = item.OrderId,
                    Price = item.Price,
                    ProductId = item.ProductId,
                    Name = item.Name,
                    Quantity = item.Quantity
                }).ToList()
            });
        }
    }
}
