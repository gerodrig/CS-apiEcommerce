using AutoMapper;
using cs_apiEcommerce.Models;
using cs_apiEcommerce.Models.Dtos;

namespace cs_apiEcommerce.Mapping;

public class ProductProfile : Profile
{
    public ProductProfile()
    {

        CreateMap<Product, ProductDto>()
        .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
        .ReverseMap();
        CreateMap<Product, CreateProductDto>().ReverseMap();
        CreateMap<Product, UpdateProductDto>().ReverseMap();
    }
}
