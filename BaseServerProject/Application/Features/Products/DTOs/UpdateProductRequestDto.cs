namespace BaseServerProject.Application.Features.Products.DTOs;

public class UpdateProductRequestDto
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

public class UpdateProductVariantDto
{
    public int? VariantID { get; set; } 
    public int ColorID { get; set; }
    public int SizeID { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int QuantityInStock { get; set; }
}