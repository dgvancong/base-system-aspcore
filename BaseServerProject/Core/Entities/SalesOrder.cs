using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseServerProject.Core.Entities;

[Table("SalesOrders")]
public class SalesOrder
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderID { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; } = 0;

    [MaxLength(50)]
    public string? PaymentMethod { get; set; } = "Tiền mặt";

    [MaxLength(50)]
    public string? OrderStatus { get; set; } = "Hoàn thành";

    [MaxLength(100)]
    public string? SalesPerson { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; } = 0;

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation property
    public virtual ICollection<SalesOrderDetail> OrderDetails { get; set; } = new List<SalesOrderDetail>();
}