namespace BaseServerProject.Application.Features.Orders.DTOs;

public class OrderDto
{
    public int OrderID { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? OrderStatus { get; set; }
    public string? SalesPerson { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? Notes { get; set; }
    public List<OrderDetailDto> OrderDetails { get; set; } = new();
}

public class OrderDetailDto
{
    public int ProductID { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
}