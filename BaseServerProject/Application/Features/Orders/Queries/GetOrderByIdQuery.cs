using MediatR;
using BaseServerProject.Application.Features.Orders.DTOs;

namespace BaseServerProject.Application.Features.Orders.Queries;

public class GetOrderByIdQuery : IRequest<OrderDto>
{
    public int OrderID { get; set; }
}