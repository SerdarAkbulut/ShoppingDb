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
                Id = x.Id,
                OrderDate = x.OrderDate,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Address = x.Address,
                Phone = x.Phone,
                UserId = x.UserId,
                DeliveryFee = x.DeliveryFee,
                SubTotal = x.SubTotal,
                OrderStatus = x.OrderStatus,
                OrderItems = x.OrderItems.Select(item => new OrderItemDTO
                {
                    Id = item.Id,
                    OrderId = item.OrderId,
                    Price = item.Price,
                    ProductId = item.ProductId,
                   ProductImage=item.ProductImages,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity
                }).ToList()
            });
        }
    }
}
