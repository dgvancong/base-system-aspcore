namespace BaseServerProject.Application.Features.Products.DTOs;

public class ProductVariantDto
{
    public int VariantID { get; set; }
    public int ColorID { get; set; }
    public string? ColorName { get; set; }
    public string? ColorCode { get; set; }
    public int SizeID { get; set; }
    public string? SizeName { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int QuantityInStock { get; set; }
    public string? Status { get; set; }
}

public class CreateProductVariantDto
{
    public int ColorID { get; set; }
    public int SizeID { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public int QuantityInStock { get; set; }
}