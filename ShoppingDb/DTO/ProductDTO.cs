using Newtonsoft.Json;
using ShoppingApi.Entity;
using ShoppingDb.Entity;

namespace ShoppingDb.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }
        public string? Discount { get; set; }
        public bool IsActive { get; set; }

        public List<ImageDTO> Images { get; set; } = new List<ImageDTO>();

        public List<ProductCategoryDTO> ProductCategories { get; set; } = new List<ProductCategoryDTO>();

        public List<ProductVariantDTO>? ProductVariants { get; set; } = new List<ProductVariantDTO>();
    }
    public class ImageDTO
    {
        public int Id { get; set; } 
        public string ImageUrl { get; set; }
    }
    public class ProductCategoryDTO
    {
        public int CategoryId { get; set; }
    }
    public class ProductVariantDTO
    {

        public int ColorId { get; set; }
         [JsonIgnore]
        public ColorDTO? Color { get; set; }



        public int SizeId { get; set; }
        [JsonIgnore]
        public SizeDTO? Size { get; set; }

        public int Stock { get; set; }
    }
    public class ColorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class SizeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class GETProducts
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Discount { get; set; }
        public string Price { get; set; }
        public List<ImageDTO> Images { get; set; } = new List<ImageDTO>();
    }

    public class ColorDto
    {
        public string Color { get; set; }
    }

}
