using MediatR;
using Microsoft.EntityFrameworkCore;
using BaseServerProject.Application.Features.Products.Commands;
using BaseServerProject.Application.Features.Products.DTOs;
using BaseServerProject.Core.Entities;
using BaseServerProject.Infrastructure.Persistence;

namespace BaseServerProject.Application.Features.Products.Handlers;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly ApplicationDbContext _context;

    public UpdateProductCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        // 1. Tìm sản phẩm cần sửa
        var product = await _context.Products
            .Include(p => p.Variants)
                .ThenInclude(v => v.Color)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Size)
            .FirstOrDefaultAsync(p => p.ProductID == request.ProductID, cancellationToken);

        if (product == null)
            throw new Exception($"Không tìm thấy sản phẩm với ID {request.ProductID}");

        // 2. Cập nhật thông tin sản phẩm
        product.ProductName = request.ProductName;
        product.SupplierName = request.SupplierName;
        product.CategoryName = request.CategoryName;
        product.Description = request.Description;
        product.BrandName = request.BrandName;
        product.Material = request.Material;
        product.Gender = request.Gender;
        product.Status = request.Status ?? product.Status;
        product.UpdatedDate = DateTime.UtcNow;

        _context.Products.Update(product);

        // 3. Xử lý các variants
        var existingVariantIds = product.Variants.Select(v => v.VariantID).ToHashSet();
        var requestVariantIds = request.Variants.Where(v => v.VariantID.HasValue).Select(v => v.VariantID.Value).ToHashSet();

        // 3.1. Xóa các variant không còn trong request
        var variantsToDelete = product.Variants.Where(v => !requestVariantIds.Contains(v.VariantID)).ToList();
        _context.ProductVariants.RemoveRange(variantsToDelete);

        // 3.2. Cập nhật hoặc thêm mới variants
        foreach (var variantDto in request.Variants)
        {
            var color = await _context.Colors.FindAsync(variantDto.ColorID);
            if (color == null)
                throw new Exception($"Màu sắc ID {variantDto.ColorID} không tồn tại");

            var size = await _context.Sizes.FindAsync(variantDto.SizeID);
            if (size == null)
                throw new Exception($"Kích thước ID {variantDto.SizeID} không tồn tại");

            if (variantDto.VariantID.HasValue)
            {
                // Cập nhật variant hiện có
                var existingVariant = product.Variants.FirstOrDefault(v => v.VariantID == variantDto.VariantID.Value);
                if (existingVariant != null)
                {
                    existingVariant.ColorID = variantDto.ColorID;
                    existingVariant.SizeID = variantDto.SizeID;
                    existingVariant.PurchasePrice = variantDto.PurchasePrice;
                    existingVariant.SellingPrice = variantDto.SellingPrice;
                    existingVariant.QuantityInStock = variantDto.QuantityInStock;
                    existingVariant.SKU = $"{product.ProductCode}-{color.ColorName}-{size.SizeName}".ToUpper();
                    existingVariant.Status = variantDto.QuantityInStock > 0 ? "Đang bán" : "Hết hàng";
                    existingVariant.UpdatedDate = DateTime.UtcNow;

                    _context.ProductVariants.Update(existingVariant);
                }
            }
            else
            {
                // Thêm mới variant
                var newVariant = new ProductVariant
                {
                    ProductID = product.ProductID,
                    ColorID = variantDto.ColorID,
                    SizeID = variantDto.SizeID,
                    SKU = $"{product.ProductCode}-{color.ColorName}-{size.SizeName}".ToUpper(),
                    PurchasePrice = variantDto.PurchasePrice,
                    SellingPrice = variantDto.SellingPrice,
                    QuantityInStock = variantDto.QuantityInStock,
                    Status = variantDto.QuantityInStock > 0 ? "Đang bán" : "Hết hàng",
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };
                _context.ProductVariants.Add(newVariant);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        // 4. Lấy lại sản phẩm đã cập nhật
        var updatedProduct = await _context.Products
            .Include(p => p.Variants)
                .ThenInclude(v => v.Color)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Size)
            .FirstOrDefaultAsync(p => p.ProductID == product.ProductID, cancellationToken);

        return MapToProductDto(updatedProduct!);
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