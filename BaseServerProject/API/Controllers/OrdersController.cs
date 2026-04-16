using MediatR;
using Microsoft.AspNetCore.Mvc;
using BaseServerProject.Application.Common.Base;
using BaseServerProject.Application.Features.Orders.Commands;
using BaseServerProject.Application.Features.Orders.DTOs;
using BaseServerProject.Application.Features.Orders.Queries;

namespace BaseServerProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateOrder([FromBody] CreateOrderRequestDto request)
    {
        var command = new CreateOrderCommand
        {
            PaymentMethod = request.PaymentMethod,
            SalesPerson = request.SalesPerson,
            DiscountAmount = request.DiscountAmount,
            Notes = request.Notes,
            Items = request.Items
        };

        var orderId = await _mediator.Send(command);
        return Ok(new { success = true, orderId, message = "Tạo đơn hàng thành công" });
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderDto>>> GetOrders(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? orderStatus,
        [FromQuery] string? salesPerson,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetOrderListQuery
        {
            FromDate = fromDate,
            ToDate = toDate,
            OrderStatus = orderStatus,
            SalesPerson = salesPerson,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(int id)
    {
        var query = new GetOrderByIdQuery { OrderID = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}