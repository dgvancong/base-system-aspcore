using MediatR;
using BaseServerProject.Application.Features.Orders.DTOs;

namespace BaseServerProject.Application.Features.Orders.Commands;

public class CreateOrderCommand : IRequest<int>
{
    public string? PaymentMethod { get; set; }
    public string? SalesPerson { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? Notes { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}