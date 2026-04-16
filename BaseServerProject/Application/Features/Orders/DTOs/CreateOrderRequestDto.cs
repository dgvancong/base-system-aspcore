namespace BaseServerProject.Application.Features.Orders.DTOs;

public class CreateOrderRequestDto
{
    public string? PaymentMethod { get; set; } = "Tiền mặt";
    public string? SalesPerson { get; set; }
    public decimal DiscountAmount { get; set; } = 0;
    public string? Notes { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public int ProductID { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; } = 0;
}