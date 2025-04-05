using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.Data;

namespace ShoppingApi.Controllers
{
    [ApiController]
    [Route("/api/products")]
    public class ProductController : ControllerBase
        
    {
        private readonly DataContext _context;
        public ProductController(DataContext context) 
        {
            _context = context;
        }
        [HttpGet]
        public async Task< IActionResult> GetProducts()
        {
            var products= await _context.Products.Include(i=>i.Images).ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task <IActionResult> GetProduct(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product= await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) {
                return NotFound();
            }
            return Ok(product); 
        }
       
    }
}
