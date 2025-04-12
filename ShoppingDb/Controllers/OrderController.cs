using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.Data;
using ShoppingApi.DTO;
using ShoppingApi.Entity;
using ShoppingApi.extensions;
namespace ShoppingApi.Controllers
{
    [Route("api/order")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly DataContext _context;
        public OrderController(DataContext context)
        {

            _context = context;
        }
        [HttpGet("getorders")]
        public async Task<ActionResult<List<OrderDTO>>> GetOrders()
        {
            return await _context.Orders.Include(x => x.OrderItems)
         .OrderToDTO()
                .Where(x => x.UserId == User.Identity!.Name)
                .ToListAsync(); ;
        }

        [HttpGet("{id}", Name = "getorder")]
        public async Task<ActionResult<List<OrderDTO>>> GetOrder(int id)
        {
            return await _context.Orders.Include(x => x.OrderItems)
                .OrderToDTO()
                .Where(x => x.UserId == User.Identity!.Name && x.Id == id).ToListAsync(); ;
        }

        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDTO createOrderDTO)
        {
            var cart = await _context.Carts.Include(i => i.CartItems)
            .ThenInclude(i => i.Product)
            .Where(i => i.userId == User.Identity!.Name).FirstOrDefaultAsync();

            if (cart == null || cart.CartItems.Count == 0)
            {
                return BadRequest("Your cart is empty");
            }
            var items = new List<OrderItem>();

            foreach (var item in cart.CartItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Price = product.Price,
                    ProductImage = product.Images.FirstOrDefault().ImageUrl,
                    ProductName = product.Name,
                    Quantity = item.Quantity
                };
                items.Add(orderItem);
                // product.Stock -= item.Quantity;
                // if (product.Stock < 0)
                // {
                //     return BadRequest($"Product {product.Name} is out of stock");
                // }
            }
            var subTotal = items.Sum(i => i.Price * i.Quantity);
            var deliveryFee = 0;

            var order = new Order
            {
                OrderItems = items,
                UserId = User.Identity!.Name,
                FirstName = createOrderDTO.FirstName,
                LastName = createOrderDTO.LastName,
                Address = createOrderDTO.Address,
                Phone = createOrderDTO.Phone,
                SubTotal = subTotal,
                DeliveryFee = deliveryFee,
            };

            _context.Orders.Add(order);
            _context.Carts.Remove(cart);

            var result = await _context.SaveChangesAsync() > 0;
            if (result)
            {
                return CreatedAtRoute(nameof(GetOrder), new { id = order.Id }, order.Id);
            }
            else
            {
                return BadRequest("Failed to create order");
            }
        }
    }
}
