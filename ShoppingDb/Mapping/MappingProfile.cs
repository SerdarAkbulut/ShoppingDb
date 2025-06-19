using AutoMapper;
using ShoppingApi.Entity;
using ShoppingDb.DTO;
using ShoppingDb.Entity;
using System.Globalization;

namespace ShoppingApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDTO>();
            CreateMap<Image, ImageDTO>();
            CreateMap<ProductCategory, ProductCategoryDTO>();
            CreateMap<ProductVariant, ProductVariantDTO>();
            CreateMap<Color, ColorDTO>();
            CreateMap<Size, SizeDTO>();
            CreateMap<Product, GETProducts>();
    
        }
    }
}
