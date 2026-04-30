using MediatR;
using Microsoft.EntityFrameworkCore;
using BaseServerProject.Application.Common.Interfaces;
using BaseServerProject.Application.Features.Products.Commands;
using BaseServerProject.Infrastructure.Persistence;

namespace BaseServerProject.Application.Features.Products.Handlers;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly ApplicationDbContext _context;
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(ApplicationDbContext context, IProductRepository productRepository)
    {
        _context = context;
        _productRepository = productRepository;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.ProductID == request.ProductID, cancellationToken);

        if (product == null)
            return false;

        // Kiểm tra xem sản phẩm có trong đơn hàng không
        var hasOrders = await _context.SalesOrderDetails
            .AnyAsync(d => d.ProductID == request.ProductID, cancellationToken);

        if (hasOrders)
        {
            // Đã có đơn hàng -> Soft delete
            product.SoftDelete();
        }
        else
        {
            // Chưa có đơn hàng -> Xóa cứng
            _context.Products.Remove(product);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}