using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseServerProject.Core.Entities;

[Table("SalesOrderDetails")]
public class SalesOrderDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderDetailID { get; set; }

    public int OrderID { get; set; }

    public int ProductID { get; set; }

    [Required]
    [MaxLength(50)]
    public string? ProductCode { get; set; }

    [Required]
    [MaxLength(200)]
    public string? ProductName { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; } = 0;

    // Navigation properties
    [ForeignKey(nameof(OrderID))]
    public virtual SalesOrder? Order { get; set; }

    [ForeignKey(nameof(ProductID))]
    public virtual Product? Product { get; set; }
}