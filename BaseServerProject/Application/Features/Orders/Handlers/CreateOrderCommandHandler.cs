using MediatR;
using Microsoft.EntityFrameworkCore;
using BaseServerProject.Application.Features.Orders.Commands;
using BaseServerProject.Core.Entities;
using BaseServerProject.Infrastructure.Persistence;

namespace BaseServerProject.Application.Features.Orders.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
{
    private readonly ApplicationDbContext _context;

    public CreateOrderCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Tạo đơn hàng mới
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
                // Tìm variant theo VariantID - Dùng DbSet ProductVariants
                var variant = await _context.ProductVariants
                      .Include(v => v.Product)
                      .Include(v => v.Color)
                      .Include(v => v.Size)
                      .FirstOrDefaultAsync(v => v.VariantID == item.VariantID, cancellationToken);

                if (variant == null)
                    throw new Exception($"Không tìm thấy biến thể sản phẩm với VariantID = {item.VariantID}");

                // Kiểm tra tồn kho
                if (item.Quantity > variant.QuantityInStock)
                {
                    throw new Exception(
                        $"Sản phẩm {variant.Product?.ProductName} " +
                        $"({variant.Color?.ColorName} - {variant.Size?.SizeName}) " +
                        $"không đủ tồn kho. Tồn kho: {variant.QuantityInStock}"
                    );
                }

                // Cập nhật tồn kho
                variant.QuantityInStock -= item.Quantity;
                if (variant.QuantityInStock <= 0)
                {
                    variant.Status = "Hết hàng";
                }
                variant.UpdatedDate = DateTime.UtcNow;

                _context.ProductVariants.Update(variant);

                var detailTotal = (item.UnitPrice * item.Quantity) - item.DiscountAmount;
                orderTotal += detailTotal;

                // Tạo chi tiết đơn hàng
                var orderDetail = new SalesOrderDetail
                {
                    OrderID = order.OrderID,
                    VariantID = variant.VariantID,
                    ProductID = variant.ProductID,
                    ProductCode = variant.Product?.ProductCode ?? "",
                    ProductName = variant.Product?.ProductName ?? "",
                    ColorName = variant.Color?.ColorName,
                    SizeName = variant.Size?.SizeName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountAmount = item.DiscountAmount,
                    TotalAmount = detailTotal
                };

                _context.SalesOrderDetails.Add(orderDetail);
            }

            // Cập nhật tổng tiền đơn hàng
            order.TotalAmount = orderTotal - request.DiscountAmount;

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return order.OrderID;
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            var innerMessage = ex.InnerException?.Message;
            throw new Exception($"Lỗi database: {innerMessage ?? ex.Message}", ex);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}