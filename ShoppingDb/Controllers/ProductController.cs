using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.Data;
using ShoppingApi.Entity;
using ShoppingDb.DTO;
using ShoppingDb.Entity;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using dotenv.net;
using System.Text.RegularExpressions;
using System.Net;
using System.Globalization;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
namespace ShoppingApi.Controllers
{
    [ApiController]
    [Route("/api/products")]

    public class ProductController : ControllerBase

    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public ProductController(DataContext context,IMapper mapper, UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
       _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int page)
        {
            int pageSize = 12;

            var products = await _context.Products
                .OrderByDescending(p => p.Id)
                .Include(p => p.ProductCategories)
                .Include(p => p.Images)
                .Skip((page - 1) * pageSize)
                .Take(pageSize + 1) // +1 alarak devamı olup olmadığını kontrol edeceğiz
                .ToListAsync();

            bool hasNextPage = products.Count > pageSize;

            if (hasNextPage)
                products.RemoveAt(pageSize);

            var productDTOs = _mapper.Map<List<GETProducts>>(products);

            return Ok(new
            {
                products = productDTOs,
                hasNextPage = hasNextPage
            });
        }

        [HttpGet("admin-products")]
        public async Task<IActionResult> GetProductsForAdmin()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if(role != "Admin")
            {
                return BadRequest();
            }

            var products = await _context.Products
                 .Include(i => i.ProductCategories)
                     .ThenInclude(i => i.Category)
                 .Select(i => new
                 {
                     i.Id,
                     i.Name,
                     price = i.Price.ToString("N2", new CultureInfo("tr-TR")),
                     i.Description,
                     productVariants = i.ProductVariants.Select(v => new { color = v.Color.Name, colorId = v.Color.Id, v.Stock, sizeId = v.Size.Id, size = v.Size.Name, v.Id }).ToList(),
                     Images = i.Images.Select(img => new { img.Id, img.ImageUrl }).ToList(),
                     ProductCategories = i.ProductCategories.Select(pc => new { pc.Category.Name, pc.CategoryId, pc.Id }).ToList(),
                     i.IsActive
                     
                 })
                 .ToListAsync();


            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products
                .Where(p => p.Id == id)
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Discount,

                    Categories = p.ProductCategories.Select(pc => new
                    {
                        pc.Category.Id,
                        pc.Category.Name
                    }),
                    p.Description,
                    price = p.Price,
                    ProductVariants = p.ProductVariants.Select(pv => new
                    {
                        pv.Id,
                        pv.Color,
                        pv.Size
                    }),
                    Images = p.Images.Select(pi => new 
                    { 
pi.ImageUrl                    
                    })
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        [HttpGet("product-variant")]
        public async Task<IActionResult> GetVariants()
        {

            var Categories = await _context.Categories.Select(i => new { i.Name, i.Id }).ToListAsync();
            var Colors = await _context.Colors.ToListAsync();
            var Sizes = await _context.Sizes.ToListAsync();
            return Ok(new { Categories, Colors, Sizes });
        }



        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO product)
        {
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            var cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            cloudinary.Api.Secure = true;

            var imageList = new List<Image>();

            foreach (var img in product.Images)
            {
                var base64Data = Regex.Match(img.ImageUrl, @"data:image/(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;
                var bytes = Convert.FromBase64String(base64Data);

                using (var stream = new MemoryStream(bytes))
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription("image.jpg", stream),
                        UseFilename = true,
                        UniqueFilename = true,
                        Overwrite = false
                    };

                    var uploadResult = cloudinary.Upload(uploadParams);

                    imageList.Add(new Image
                    {
                        ImageUrl = uploadResult.SecureUrl.ToString(),
                        PublicId = uploadResult.PublicId,
                        
                    });
                }
            }

            var newProduct = new Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                IsActive =true,
                ProductVariants = product.ProductVariants.Select(pv => new ProductVariant
                {
                    SizeId = pv.SizeId,
                    ColorId = pv.ColorId,
                    Stock = pv.Stock
                }).ToList(),
                ProductCategories = product.ProductCategories.Select(c => new ProductCategory
                {
                    CategoryId = c.CategoryId
                }).ToList(),
                Images = imageList
            };

            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            return Ok(newProduct);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            // Cloudinary'den resimleri sil
            foreach (var image in product.Images)
            {
                if (!string.IsNullOrEmpty(image.PublicId))
                {
                    var cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
                    cloudinary.Api.Secure = true;
                    var deletionParams = new DeletionParams(image.PublicId);
                    var deletionResult = cloudinary.Destroy(deletionParams);

                    // Burada hata kontrolü yapabilirsiniz.
                    if (deletionResult.StatusCode != HttpStatusCode.OK)
                    {
                        return StatusCode(500, new { message = "Resim silme işlemi başarısız oldu." });
                    }
                }
            }

            // Ürünü ve resimleri veritabanından sil
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ürün ve görseller başarıyla silindi." });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO? productDTO)
        {
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            var cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            cloudinary.Api.Secure = true;

            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.ProductVariants)
                .Include(p => p.ProductCategories)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            // 1. Eski resimleri DTO'da olmayanlara göre sil
            var dtoIds = productDTO.Images.Where(i => i.Id != 0).Select(i => i.Id).ToList();
            var imagesToDelete = product.Images.Where(img => !dtoIds.Contains(img.Id)).ToList();

            foreach (var img in imagesToDelete)
            {
                cloudinary.Destroy(new DeletionParams(img.PublicId));
                _context.Images.Remove(img);
            }

            // 2. Yeni base64 resimleri Cloudinary'e yükle
            var newImageDTOs = productDTO.Images.Where(i => i.Id == 0);
            var newImages = new List<Image>();

            foreach (var img in newImageDTOs)
            {
                var base64Data = Regex.Match(img.ImageUrl, @"data:image/(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;
                var bytes = Convert.FromBase64String(base64Data);

                using (var stream = new MemoryStream(bytes))
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription("image.jpg", stream),
                        UseFilename = true,
                        UniqueFilename = true,
                        Overwrite = false
                    };

                    var uploadResult = cloudinary.Upload(uploadParams);

                    newImages.Add(new Image
                    {
                        ImageUrl = uploadResult.SecureUrl.ToString(),
                        PublicId = uploadResult.PublicId,
                    });
                }
            }

            // 3. Ürünün tüm bilgilerini güncelle
            product.Name = productDTO.Name;
            product.Description = productDTO.Description;
            product.Price = productDTO.Price;
            product.IsActive = productDTO.IsActive;

            // Eski variant ve kategori bilgilerini temizle
            _context.ProductVariants.RemoveRange(product.ProductVariants);
            _context.ProductCategories.RemoveRange(product.ProductCategories);

            // Yeni bilgileri ata
            product.ProductVariants = productDTO.ProductVariants.Select(pv => new ProductVariant
            {
                SizeId = pv.SizeId,
                ColorId = pv.ColorId,
                Stock = pv.Stock
            }).ToList();

            product.ProductCategories = productDTO.ProductCategories.Select(c => new ProductCategory
            {
                CategoryId = c.CategoryId
            }).ToList();

            product.Images.AddRange(newImages); // sadece yeni resimleri ekle (silinenler zaten kaldırıldı)

            await _context.SaveChangesAsync();

            return Ok(product);
        }

        [HttpGet("category/{catId}/{page}")]
        public async Task<IActionResult> GetCategoryProducts(int catId, int page)
        {
            var query = _context.Products
                .Where(p => p.ProductCategories.Any(pc => pc.CategoryId == catId))
                .Select(i => new
                {
                    i.Id,
                    i.Name,
                    price = i.Price.ToString("N2", new CultureInfo("tr-TR")),
                    i.Description,
                    Images = i.Images.Select(img => new { img.Id, img.ImageUrl }).ToList(),
                    i.Discount
                });

            var products = await query
                .Skip((page - 1) * 2)
                .Take(2+1) 
                .ToListAsync();

            bool hasNextPage = products.Count > 2;

            if (hasNextPage)
                products.RemoveAt(2); 

            return Ok(new
            {
                products,
                hasNextPage
            });
        }

        [HttpGet("home")]
        public async Task<IActionResult> GetHomeProducts()
        {
            var products = await _context.Products
                .Where(p=>p.IsActive==true).OrderByDescending(p=>p.Id)
                  .Include(i => i.ProductCategories)
                    .Include(p=>p.Images)
            
                  .ToListAsync();
            var productDTOs = _mapper.Map<List<GETProducts>>(products);

            return Ok(productDTOs);
        }
        [HttpGet("last")]
        public async Task<IActionResult> GetLastProducts()
        {
            var products = await _context.Products
               .Include(p => p.ProductCategories)
                .Include(p => p.Images)
                  .OrderByDescending(p=>p.Id).Take(5)
                  .ToListAsync();
                  var productDTOs=_mapper.Map<List<GETProducts>>(products);

            return Ok(productDTOs);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string q)
        {
            var products = await _context.Products
                .Include(p => p.ProductCategories)
                .Include(p => p.Images)
                .Where(p => p.Name.ToLower().Contains(q.ToLower()) ||  p.Description.ToLower().Contains(q.ToLower())) 
                .ToListAsync();

            var productDTOs = _mapper.Map<List<GETProducts>>(products);
            return Ok(productDTOs);
        }
        [HttpPost("add-best-sellers/{id}")]
        public async Task<IActionResult> AddBestSellers(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { message = "Ürün bulunamadı." });

            product.IsActive = !product.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ürün durumu güncellendi.", product });
        }
        [HttpPut("add-discount/{id}/{discount}")]
        public async Task<IActionResult> AddDiscount(int id,string discount)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { message = "Ürün Bulunamadı" });
            product.Discount =   discount;
            _context.SaveChangesAsync();
            return Ok(product);
            
        }
        [HttpDelete("delete-discount/{id}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { message = "Ürün Bulunamadı" });
            product.Discount = null;
            _context.SaveChangesAsync();

            return Ok(product);

        }
    }
}
