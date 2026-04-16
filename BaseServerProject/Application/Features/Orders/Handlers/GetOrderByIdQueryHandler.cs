using MediatR;
using Microsoft.EntityFrameworkCore;
using BaseServerProject.Application.Features.Orders.DTOs;
using BaseServerProject.Application.Features.Orders.Queries;
using BaseServerProject.Infrastructure.Persistence;

namespace BaseServerProject.Application.Features.Orders.Handlers;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly ApplicationDbContext _context;

    public GetOrderByIdQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _context.SalesOrders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderID == request.OrderID, cancellationToken);

        if (order == null)
        {
            throw new Exception($"Không tìm thấy đơn hàng với ID {request.OrderID}");
        }

        // Map thủ công
        var orderDto = new OrderDto
        {
            OrderID = order.OrderID,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            PaymentMethod = order.PaymentMethod,
            OrderStatus = order.OrderStatus,
            SalesPerson = order.SalesPerson,
            DiscountAmount = order.DiscountAmount,
            Notes = order.Notes,
            OrderDetails = order.OrderDetails?.Select(d => new OrderDetailDto
            {
                ProductID = d.ProductID,
                ProductCode = d.ProductCode ?? string.Empty,
                ProductName = d.ProductName ?? string.Empty,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                DiscountAmount = d.DiscountAmount,
                TotalAmount = d.TotalAmount
            }).ToList() ?? new List<OrderDetailDto>()
        };

        return orderDto;
    }
}