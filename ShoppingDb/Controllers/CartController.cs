using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.Data;
using ShoppingApi.DTO;
using ShoppingApi.Entity;
using System.Security.Claims;

namespace ShoppingApi.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly DataContext _context;

        public CartController(DataContext context)
        {
            _context = context;
        }

     
        [HttpGet]
        public async Task<ActionResult<CartDTO>> GetCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var cart = await GetOrCreate(userId);
            return CartToDTO(cart);
        }


    

        [HttpPost("add")]
        public async Task<ActionResult> AddItemToCart(int productId, int quantity, int colorId, int sizeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var cart = await GetOrCreate(userId);

            var product = await _context.Products.FirstOrDefaultAsync(i => i.Id == productId);
            if (product == null)
                return NotFound("The product is not in the database");
          
            cart.AddItem(product, quantity, colorId, sizeId);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
                return CreatedAtAction(nameof(GetCart), new { userId }, CartToDTO(cart));

            return BadRequest(new ProblemDetails { Title = "The product could not be added to the cart" });
        }

        [HttpDelete("deleteItem")]
        [Authorize]
        public async Task<ActionResult> DeleteFromCart(int productId, int quantity, int colorId, int sizeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
        return Unauthorized();
            var cart = await GetOrCreate(userId);

            cart.DeleteItem(productId, quantity,colorId,sizeId);
            var result = await _context.SaveChangesAsync() > 0;

            if (result)
                return Ok(CartToDTO(cart));

            return BadRequest(new ProblemDetails { Title = "The product could not be removed from the cart" });
        }

        
        private async Task<Cart> GetOrCreate(string userId)
        {
            var cart = await _context.Carts
                .Include(i => i.CartItems)
                    .ThenInclude(ci => ci.Product)
                .Include(i => i.CartItems)
                    .ThenInclude(ci => ci.color)
                .Include(i => i.CartItems)
                    .ThenInclude(ci => ci.size)
                .FirstOrDefaultAsync(i => i.userId == userId);

            if (cart == null)
            {
                cart = new Cart { userId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

     
        private CartDTO CartToDTO(Cart cart)
        {
            return new CartDTO
            {
                CartId = cart.CartId,
                userId = cart.userId,
                CartItems = cart.CartItems.Select(item => new CartItemDTO
                {
                    ProductId = item.ProductId,
                    Name = item.Product?.Name,
                    Price = item.Product?.Price,
                    Quantity = item.Quantity,
                    ImageUrl = item.Product?.Images.FirstOrDefault()?.ImageUrl,
                   color=item.color,
                   size=item.size,
                   IsCargoFree=item.Product.IsCargoFree
                   
                   
                }).ToList()
            };
        }


    }

}
