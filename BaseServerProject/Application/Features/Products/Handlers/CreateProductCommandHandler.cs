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
        // 1. Kiểm tra mã sản phẩm đã tồn tại
        var existingProduct = await _context.Products
            .FirstOrDefaultAsync(p => p.ProductCode == request.ProductCode, cancellationToken);

        if (existingProduct != null)
            throw new Exception($"Mã sản phẩm {request.ProductCode} đã tồn tại");

        // 2. Tạo sản phẩm mới
        var product = new Product
        {
            ProductCode = request.ProductCode,
            ProductName = request.ProductName,
            SupplierName = request.SupplierName,
            CategoryName = request.CategoryName,
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

        // 3. Tạo các biến thể (màu sắc + kích thước)
        foreach (var variantDto in request.Variants)
        {
            // Kiểm tra màu sắc tồn tại
            var color = await _context.Colors.FindAsync(variantDto.ColorID);
            if (color == null)
                throw new Exception($"Màu sắc ID {variantDto.ColorID} không tồn tại");

            // Kiểm tra kích thước tồn tại
            var size = await _context.Sizes.FindAsync(variantDto.SizeID);
            if (size == null)
                throw new Exception($"Kích thước ID {variantDto.SizeID} không tồn tại");

            // Tạo SKU: {ProductCode}-{ColorName}-{SizeName}
            var sku = $"{product.ProductCode}-{color.ColorName}-{size.SizeName}".ToUpper();

            var variant = new ProductVariant
            {
                ProductID = product.ProductID,
                ColorID = variantDto.ColorID,
                SizeID = variantDto.SizeID,
                SKU = sku,
                PurchasePrice = variantDto.PurchasePrice,
                SellingPrice = variantDto.SellingPrice,
                QuantityInStock = variantDto.QuantityInStock,
                Status = variantDto.QuantityInStock > 0 ? "Đang bán" : "Hết hàng",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            _context.ProductVariants.Add(variant);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // 4. Lấy lại sản phẩm kèm variants để trả về
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
                SKU = v.SKU,
                PurchasePrice = v.PurchasePrice,
                SellingPrice = v.SellingPrice,
                QuantityInStock = v.QuantityInStock,
                Status = v.Status
            }).ToList()
        };
    }
}