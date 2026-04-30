using BaseServerProject.Application.Features.Products.DTOs;
using BaseServerProject.Application.Features.Products.Queries;
using BaseServerProject.Core.Entities;
using BaseServerProject.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseServerProject.Application.Features.Products.Handlers;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly ApplicationDbContext _context;

    public GetProductByIdQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Include(p => p.Variants)
                .ThenInclude(v => v.Color)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Size)
            .FirstOrDefaultAsync(p => p.ProductID == request.ProductID, cancellationToken);

        if (product == null)
            return null;

        return MapToProductDto(product);
    }

    private ProductDto MapToProductDto(Product product)
    {
        return new ProductDto
        {
            ProductID = product.ProductID,
            ProductCode = product.ProductCode,
            ProductName = product.ProductName,
            SupplierName = product.SupplierName,
            CategoryName = product.CategoryName,
            PurchasePrice = product.PurchasePrice,
            SellingPrice = product.SellingPrice,
            Description = product.Description,
            BrandName = product.BrandName,
            Material = product.Material,
            Gender = product.Gender,
            Status = product.Status,
            CreatedDate = product.CreatedDate,
            UpdatedDate = product.UpdatedDate,
            Variants = product.Variants.Select(v => new ProductVariantDto
            {
                VariantID = v.VariantID,
                ColorID = v.ColorID,
                ColorName = v.Color?.ColorName,
                ColorCode = v.Color?.ColorCode,
                SizeID = v.SizeID,
                SizeName = v.Size?.SizeName,
                QuantityInStock = v.QuantityInStock,
                Status = v.Status
            }).ToList()
        };
    }
}