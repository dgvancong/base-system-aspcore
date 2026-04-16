using MediatR;
using Microsoft.EntityFrameworkCore;
using BaseServerProject.Application.Common.Base;
using BaseServerProject.Application.Features.Orders.DTOs;
using BaseServerProject.Application.Features.Orders.Queries;
using BaseServerProject.Infrastructure.Persistence;

namespace BaseServerProject.Application.Features.Orders.Handlers;

public class GetOrderListQueryHandler : IRequestHandler<GetOrderListQuery, PagedResult<OrderDto>>
{
    private readonly ApplicationDbContext _context;

    public GetOrderListQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<OrderDto>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
    {
        // 1. Khởi tạo query
        var query = _context.SalesOrders
            .Include(o => o.OrderDetails)
            .AsQueryable();

        // 2. Filter by date range
        if (request.FromDate.HasValue)
            query = query.Where(o => o.OrderDate >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            query = query.Where(o => o.OrderDate <= request.ToDate.Value);

        // 3. Filter by status
        if (!string.IsNullOrWhiteSpace(request.OrderStatus))
            query = query.Where(o => o.OrderStatus == request.OrderStatus);

        // 4. Filter by sales person
        if (!string.IsNullOrWhiteSpace(request.SalesPerson))
            query = query.Where(o => o.SalesPerson == request.SalesPerson);

        // 5. Order by latest first
        query = query.OrderByDescending(o => o.OrderDate);

        // 6. Lấy tổng số bản ghi
        var totalCount = await query.CountAsync(cancellationToken);

        // 7. Phân trang
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // 8. Map thủ công (không dùng AutoMapper)
        var orderDtos = items.Select(o => new OrderDto
        {
            OrderID = o.OrderID,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            PaymentMethod = o.PaymentMethod,
            OrderStatus = o.OrderStatus,
            SalesPerson = o.SalesPerson,
            DiscountAmount = o.DiscountAmount,
            Notes = o.Notes,
            OrderDetails = o.OrderDetails?.Select(d => new OrderDetailDto
            {
                ProductID = d.ProductID,
                ProductCode = d.ProductCode ?? string.Empty,
                ProductName = d.ProductName ?? string.Empty,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                DiscountAmount = d.DiscountAmount,
                TotalAmount = d.TotalAmount
            }).ToList() ?? new List<OrderDetailDto>()
        }).ToList();

        // 9. Trả về kết quả
        return new PagedResult<OrderDto>
        {
            Items = orderDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}