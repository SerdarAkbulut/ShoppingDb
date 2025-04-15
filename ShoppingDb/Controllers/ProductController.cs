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
        public async Task<IActionResult> GetProducts()
        {
            
            var products = await _context.Products
                .Include(i => i.ProductCategories)
                    .ThenInclude(i => i.Category)
                .Select(i => new
                {
                    i.Id,
                    i.Name,
                    price = i.Price.ToString("N2", new CultureInfo("tr-TR")),
                    i.Description,
                    productVariants = i.ProductVariants.Select(v => new { color=v.Color.Name, colorId = v.Color.Id, v.Stock, sizeId = v.Size.Id, size=v.Size.Name, v.Id }).ToList(),
                    Images = i.Images.Select(img => new { img.Id, img.ImageUrl }).ToList(),
                    ProductCategories = i.ProductCategories.Select(pc => new { pc.Category.Name,pc.CategoryId ,pc.Id}).ToList()
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
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
                IsActive = product.IsActive,
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
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO productDTO)
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

    }
}
