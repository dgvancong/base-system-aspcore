using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseServerProject.Core.Entities;

[Table("Products")]
public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductID { get; set; }

    [Required]
    [MaxLength(50)]
    public string ProductCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string SupplierName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MaxLength(100)]
    public string? BrandName { get; set; }

    [MaxLength(100)]
    public string? Material { get; set; }

    [MaxLength(20)]
    public string? Gender { get; set; }

    [MaxLength(20)]
    public string? Status { get; set; } = "Đang bán";

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public void UpdateStock(int quantity)
    {
        foreach (var variant in Variants)
        {
            variant.UpdateStock(quantity);
        }

        UpdateProductStatus();
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdateVariantStock(int colorId, int sizeId, int quantity)
    {
        var variant = Variants.FirstOrDefault(v => v.ColorID == colorId && v.SizeID == sizeId);
        if (variant != null)
        {
            variant.UpdateStock(quantity);
            UpdateProductStatus();
            UpdatedDate = DateTime.UtcNow;
        }
    }

    private void UpdateProductStatus()
    {
        var totalStock = Variants.Sum(v => v.QuantityInStock);

        if (totalStock <= 0)
        {
            Status = "Hết hàng";
        }
        else if (Status == "Hết hàng")
        {
            Status = "Đang bán";
        }
    }

    public bool IsAvailable()
    {
        return Status == "Đang bán" && Variants.Any(v => v.QuantityInStock > 0);
    }

    public bool IsVariantAvailable(int colorId, int sizeId)
    {
        var variant = Variants.FirstOrDefault(v => v.ColorID == colorId && v.SizeID == sizeId);
        return variant != null && variant.IsAvailable();
    }

    public int GetTotalStock()
    {
        return Variants.Sum(v => v.QuantityInStock);
    }

    public decimal GetMinPrice()
    {
        return Variants.Any() ? Variants.Min(v => v.SellingPrice) : 0;
    }

    public decimal GetMaxPrice()
    {
        return Variants.Any() ? Variants.Max(v => v.SellingPrice) : 0;
    }
}