using MediatR;
using BaseServerProject.Application.Features.Products.DTOs;

namespace BaseServerProject.Application.Features.Products.Commands;

public class CreateProductCommand : IRequest<ProductDto>
{
    public string? ProductCode { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? SupplierName { get; set; }
    public string? CategoryName { get; set; }
    public string? Description { get; set; }
    public string? BrandName { get; set; }
    public decimal PurchasePrice { get; set; }      
    public decimal SellingPrice { get; set; }       
    public string? Material { get; set; }
    public string? Gender { get; set; }
    public List<CreateProductVariantDto> Variants { get; set; } = new();
}