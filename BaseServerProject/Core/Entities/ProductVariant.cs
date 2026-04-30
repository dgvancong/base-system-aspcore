using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseServerProject.Core.Entities;

[Table("ProductVariants")]
public class ProductVariant
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int VariantID { get; set; }

    public int ProductID { get; set; }
    public int ColorID { get; set; }
    public int SizeID { get; set; }

    public int QuantityInStock { get; set; } = 0;

    [MaxLength(20)]
    public string? Status { get; set; } = "Đang bán";

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(ProductID))]
    public virtual Product? Product { get; set; }

    [ForeignKey(nameof(ColorID))]
    public virtual Color? Color { get; set; }

    [ForeignKey(nameof(SizeID))]
    public virtual Size? Size { get; set; }

    public void UpdateStock(int quantity)
    {
        QuantityInStock += quantity;

        if (QuantityInStock <= 0)
        {
            Status = "Hết hàng";
        }
        else if (Status == "Hết hàng")
        {
            Status = "Đang bán";
        }

        UpdatedDate = DateTime.UtcNow;
    }

    public bool IsAvailable()
    {
        return Status == "Đang bán" && QuantityInStock > 0;
    }

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Số lượng phải lớn hơn 0");

        if (QuantityInStock < quantity)
            throw new InvalidOperationException($"Không đủ hàng. Tồn kho: {QuantityInStock}, yêu cầu: {quantity}");

        QuantityInStock -= quantity;

        if (QuantityInStock <= 0)
        {
            Status = "Hết hàng";
        }

        UpdatedDate = DateTime.UtcNow;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Số lượng phải lớn hơn 0");

        QuantityInStock += quantity;

        if (Status == "Hết hàng" && QuantityInStock > 0)
        {
            Status = "Đang bán";
        }

        UpdatedDate = DateTime.UtcNow;
    }
}