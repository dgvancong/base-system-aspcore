using MediatR;
using BaseServerProject.Application.Features.Orders.Commands;
using BaseServerProject.Core.Entities;
using BaseServerProject.Infrastructure.Persistence;
using BaseServerProject.Application.Common.Interfaces;

namespace BaseServerProject.Application.Features.Orders.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly ApplicationDbContext _context;
    private readonly IProductRepository _productRepository;

    public CreateOrderCommandHandler(ApplicationDbContext context, IProductRepository productRepository)
    {
        _context = context;
        _productRepository = productRepository;
    }

    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new SalesOrder
        {
            OrderDate = DateTime.UtcNow,
            PaymentMethod = request.PaymentMethod,
            OrderStatus = "Hoàn thành",
            SalesPerson = request.SalesPerson,
            DiscountAmount = request.DiscountAmount,
            Notes = request.Notes,
            TotalAmount = 0
        };

        _context.SalesOrders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        decimal orderTotal = 0;
        foreach (var item in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductID, cancellationToken);
            if (product == null)
                throw new Exception($"Sản phẩm ID {item.ProductID} không tồn tại");
            if (product.QuantityInStock < item.Quantity)
                throw new Exception($"Sản phẩm {product.ProductName} không đủ tồn kho");
            product.QuantityInStock -= item.Quantity;
            _context.Products.Update(product);  
            var detailTotal = (item.UnitPrice * item.Quantity) - item.DiscountAmount;
            orderTotal += detailTotal;

            var orderDetail = new SalesOrderDetail
            {
                OrderID = order.OrderID,
                ProductID = item.ProductID,
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                DiscountAmount = item.DiscountAmount,
                TotalAmount = detailTotal
            };

            _context.SalesOrderDetails.Add(orderDetail);
        }
        order.TotalAmount = orderTotal - request.DiscountAmount;
        await _context.SaveChangesAsync(cancellationToken);

        return order.OrderID;
    }
}