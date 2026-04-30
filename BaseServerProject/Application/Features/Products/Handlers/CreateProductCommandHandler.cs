using BaseServerProject.Application.Features.Products.Commands;
using BaseServerProject.Application.Features.Products.DTOs;
using BaseServerProject.Core.Entities;
using BaseServerProject.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseServerProject.Application.Features.Products.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly ApplicationDbContext _context;

    public CreateProductCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.ProductCode))
        {
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductCode == request.ProductCode, cancellationToken);

            if (existingProduct != null)
                throw new Exception($"Mã sản phẩm {request.ProductCode} đã tồn tại");
        }

        if (string.IsNullOrWhiteSpace(request.ProductName))
            throw new Exception("Tên sản phẩm là bắt buộc");

        var product = new Product
        {
            ProductCode = string.IsNullOrWhiteSpace(request.ProductCode) ? null : request.ProductCode,
            ProductName = request.ProductName,
            SupplierName = string.IsNullOrWhiteSpace(request.SupplierName) ? null : request.SupplierName,
            CategoryName = string.IsNullOrWhiteSpace(request.CategoryName) ? null : request.CategoryName,
            PurchasePrice = request.PurchasePrice,      
            SellingPrice = request.SellingPrice,       
            Description = request.Description,
            BrandName = request.BrandName,
            Material = request.Material,
            Gender = request.Gender,
            Status = "Đang bán",
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        if (request.Variants != null && request.Variants.Any())
        {
            foreach (var variantDto in request.Variants)
            {
                var color = await _context.Colors.FindAsync(variantDto.ColorID);
                if (color == null)
                    throw new Exception($"Màu sắc ID {variantDto.ColorID} không tồn tại");

                var size = await _context.Sizes.FindAsync(variantDto.SizeID);
                if (size == null)
                    throw new Exception($"Kích thước ID {variantDto.SizeID} không tồn tại");

                var sku = $"{product.ProductCode ?? "SP"}-{color.ColorName}-{size.SizeName}".ToUpper();

                var variant = new ProductVariant
                {
                    ProductID = product.ProductID,
                    ColorID = variantDto.ColorID,
                    SizeID = variantDto.SizeID,
                    QuantityInStock = variantDto.QuantityInStock,  
                    Status = variantDto.QuantityInStock > 0 ? "Đang bán" : "Hết hàng",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                _context.ProductVariants.Add(variant);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        var createdProduct = await _context.Products
            .Include(p => p.Variants)
                .ThenInclude(v => v.Color)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Size)
            .FirstOrDefaultAsync(p => p.ProductID == product.ProductID, cancellationToken);

        return MapToProductDto(createdProduct!);
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