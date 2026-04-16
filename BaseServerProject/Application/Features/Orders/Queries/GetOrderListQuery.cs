using MediatR;
using BaseServerProject.Application.Common.Base;
using BaseServerProject.Application.Features.Orders.DTOs;

namespace BaseServerProject.Application.Features.Orders.Queries;

public class GetOrderListQuery : IRequest<PagedResult<OrderDto>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? OrderStatus { get; set; }
    public string? SalesPerson { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}