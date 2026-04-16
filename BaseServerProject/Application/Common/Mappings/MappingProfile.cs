using AutoMapper;
using BaseServerProject.Application.Features.Products.DTOs;
using BaseServerProject.Application.Features.Products.Commands;
using BaseServerProject.Core.Entities;

namespace BaseServerProject.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<CreateProductCommand, Product>()
            .ForMember(dest => dest.ProductID, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore());
    }
}