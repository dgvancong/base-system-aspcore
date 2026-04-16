namespace BaseServerProject.Application.Features.Products.DTOs;

public class ProductDto
{
    public int ProductID { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? BrandName { get; set; }
    public string? Material { get; set; }
    public string? Gender { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    // Tổng hợp từ variants
    public int TotalQuantityInStock => Variants?.Sum(v => v.QuantityInStock) ?? 0;
    public decimal MinSellingPrice => Variants != null && Variants.Any()
        ? Variants.Min(v => v.SellingPrice)
        : 0;

    public decimal MaxSellingPrice => Variants != null && Variants.Any()
        ? Variants.Max(v => v.SellingPrice)
        : 0;
    public List<ProductVariantDto> Variants { get; set; } = new();
}
