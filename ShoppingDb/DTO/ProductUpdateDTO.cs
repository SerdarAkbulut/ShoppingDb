using ShoppingApi.Entity;
using ShoppingDb.DTO;

namespace ShoppingApi.DTO
{
    public class ProductUpdateDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }
        public string? Discount { get; set; }
        public bool IsActive { get; set; }
        public bool IsCargoFree { get; set; }

        public List<ImageDTO> Images { get; set; } = new List<ImageDTO>();

        public List<ProductCategoryDTO> ProductCategories { get; set; } = new List<ProductCategoryDTO>();

        public List<ProductVariantUpdateDTO>? ProductVariants { get; set; } = new List<ProductVariantUpdateDTO>();
    }

    public class ProductVariantUpdateDTO
    {
        public Color color { get; set; }
        public Size size { get; set; }
        public int Id { get; set; }
        public int stock { get; set; }
    }
  
}
