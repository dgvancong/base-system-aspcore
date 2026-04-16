namespace BaseServerProject.Application.Features.Products.DTOs;

public class CreateProductRequestDto
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string? Size { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int QuantityInStock { get; set; }
    public string? Description { get; set; }
    public string? BrandName { get; set; }
    public string? Material { get; set; }
    public string? Gender { get; set; }
}