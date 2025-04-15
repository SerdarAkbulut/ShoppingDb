using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ShoppingApi.Entity;
using ShoppingDb.Entity;

namespace ShoppingDb.DTO
{
    public class ProductDTO
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; }

        public List<ImageDTO> Images { get; set; } = new List<ImageDTO>();

        public List<ProductCategoryDTO> ProductCategories { get; set; } = new List<ProductCategoryDTO>();

        public List<ProductVariantDTO> ProductVariants { get; set; } = new List<ProductVariantDTO>();
    }
    public class ImageDTO
    {
        public int Id { get; set; } // opsiyonel ama update için gerekebilir
        public string ImageUrl { get; set; }
    }
    public class ProductCategoryDTO
    {
        public int CategoryId { get; set; }
    }
    public class ProductVariantDTO
    {
        public int SizeId { get; set; }
        public int ColorId { get; set; }
        public int Stock { get; set; }
    }
}
