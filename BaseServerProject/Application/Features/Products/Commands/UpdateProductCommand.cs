using MediatR;
using BaseServerProject.Application.Features.Products.DTOs;

namespace BaseServerProject.Application.Features.Products.Commands;

public class UpdateProductCommand : IRequest<ProductDto>
{
    public int ProductID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? BrandName { get; set; }
    public string? Material { get; set; }
    public string? Gender { get; set; }
    public string? Status { get; set; }
    public List<UpdateProductVariantDto> Variants { get; set; } = new();
}