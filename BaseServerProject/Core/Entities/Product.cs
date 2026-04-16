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

    [MaxLength(255)]
    public string? Color { get; set; }

    [MaxLength(50)]
    public string? Size { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchasePrice { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SellingPrice { get; set; }

    [Required]
    public int QuantityInStock { get; set; } = 0;

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

    // Business methods
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
}