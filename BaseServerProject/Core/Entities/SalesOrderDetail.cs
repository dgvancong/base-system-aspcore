using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseServerProject.Core.Entities;

public class SalesOrderDetail
{
    [Key]
    public int OrderDetailID { get; set; }

    public int OrderID { get; set; }

    public int VariantID { get; set; }

    public int ProductID { get; set; }

    [MaxLength(50)]
    public string ProductCode { get; set; }

    [MaxLength(200)]
    public string ProductName { get; set; }

    [MaxLength(50)]
    public string? ColorName { get; set; }

    [MaxLength(20)]
    public string? SizeName { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    // Navigation properties
    [ForeignKey("OrderID")]
    public virtual SalesOrder Order { get; set; }

    [ForeignKey("VariantID")]
    public virtual ProductVariant Variant { get; set; }

    [ForeignKey("ProductID")]
    public virtual Product Product { get; set; }
}