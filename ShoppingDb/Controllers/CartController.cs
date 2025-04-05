using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.Data;
using ShoppingApi.DTO;
using ShoppingApi.Entity;

namespace ShoppingApi.Controllers
{
    [ApiController]
    [Route("/api/cart")]
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
            var cart = await GetOrCreate();
            return CartToDTO(cart);
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddItemToCart(int productId, int quantity)
        {
            var cart = await GetOrCreate();

            var product = await _context.Products.FirstOrDefaultAsync(i => i.Id == productId);

            if (product == null)
                return NotFound("the product is not in database");

            cart.AddItem(product, quantity);

            var result = await _context.SaveChangesAsync() > 0;

            if (result)
                return CreatedAtAction(nameof(GetCart), CartToDTO(cart));

            return BadRequest(new ProblemDetails { Title = "The product can not be added to cart" });
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteFromCart(int productId,int quantity)
        {
            var cart = await GetOrCreate();
            cart.DeleteItem(productId, quantity);
            var result = await _context.SaveChangesAsync()>0;

            if (result)
            {
                return Ok(CartToDTO(cart));
            }
            return BadRequest(new ProblemDetails { Title="ürün silinemiyor"});
        }

        private async Task<Cart> GetOrCreate()
        {
            var cart = await _context.Carts
                        .Include(i => i.CartItems)
                        .ThenInclude(i => i.Product)
                        .Where(i => i.userId == Request.Cookies["userId"])
                        .FirstOrDefaultAsync();

            if (cart == null)
            {
                var userId = Guid.NewGuid().ToString();

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddMonths(1),
                    IsEssential = true
                };

                Response.Cookies.Append("userId", userId, cookieOptions);
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
                }).ToList()
            };
        }
    }
}
