using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseServerProject.Core.Entities;

[Table("Products")]
public class Product
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductID { get; set; }

    [MaxLength(50)]
    public string? ProductCode { get; set; }                    // 👈 Bỏ [Required]

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; } = string.Empty;    // 👈 Bỏ nullable

    [MaxLength(200)]
    public string? SupplierName { get; set; }

    [MaxLength(100)]
    public string? CategoryName { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchasePrice { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal SellingPrice { get; set; } = 0;

    public string? Description { get; set; }

    [MaxLength(100)]
    public string? BrandName { get; set; }

    [MaxLength(100)]
    public string? Material { get; set; }

    [MaxLength(20)]
    public string? Gender { get; set; }

    [MaxLength(20)]
    public string? Status { get; set; } = "Đang bán";

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

    // ========== BUSINESS METHODS ==========

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
        return !IsDeleted && Status == "Đang bán" && Variants.Any(v => v.QuantityInStock > 0);
    }

    public bool IsVariantAvailable(int colorId, int sizeId)
    {
        if (IsDeleted) return false;
        var variant = Variants.FirstOrDefault(v => v.ColorID == colorId && v.SizeID == sizeId);
        return variant != null && variant.IsAvailable();
    }

    public int GetTotalStock()
    {
        return Variants.Sum(v => v.QuantityInStock);
    }

    public decimal GetPrice()
    {
        return SellingPrice;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        Status = "Ngừng bán";
        UpdatedDate = DateTime.UtcNow;

        foreach (var variant in Variants)
        {
            variant.Status = "Ngừng bán";
            variant.UpdatedDate = DateTime.UtcNow;
        }
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        Status = "Đang bán";
        UpdatedDate = DateTime.UtcNow;

        foreach (var variant in Variants)
        {
            if (variant.QuantityInStock > 0)
            {
                variant.Status = "Đang bán";
            }
            variant.UpdatedDate = DateTime.UtcNow;
        }
    }
}