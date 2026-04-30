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

        // 2. Cập nhật thông tin sản phẩm (KHÔNG sửa tên và mã)
        product.SupplierName = request.SupplierName;
        product.CategoryName = request.CategoryName;
        product.PurchasePrice = request.PurchasePrice;
        product.SellingPrice = request.SellingPrice;
        product.Description = request.Description;
        product.BrandName = request.BrandName;
        product.Material = request.Material;
        product.Gender = request.Gender;
        product.Status = request.Status ?? product.Status;
        product.UpdatedDate = DateTime.UtcNow;

        _context.Products.Update(product);

        // 3. Xử lý variants
        var existingVariants = product.Variants.ToList();
        var requestVariants = request.Variants.Where(v => v.VariantID.HasValue).Select(v => v.VariantID.Value).ToList();

        // 3.1. Xử lý variants bị xóa (có trong DB nhưng không có trong request)
        var variantsToDelete = existingVariants.Where(v => !requestVariants.Contains(v.VariantID)).ToList();

        foreach (var variant in variantsToDelete)
        {
            // Kiểm tra variant có trong đơn hàng không
            var hasOrders = await _context.SalesOrderDetails
                .AnyAsync(d => d.VariantID == variant.VariantID, cancellationToken);

            if (hasOrders)
            {
                // Nếu đã có đơn hàng, chỉ cập nhật trạng thái và số lượng = 0
                variant.Status = "Ngừng bán";
                variant.QuantityInStock = 0;
                variant.UpdatedDate = DateTime.UtcNow;
                _context.ProductVariants.Update(variant);
            }
            else
            {
                // Nếu chưa có đơn hàng, xóa bình thường
                _context.ProductVariants.Remove(variant);
            }
        }

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
                var existingVariant = existingVariants.FirstOrDefault(v => v.VariantID == variantDto.VariantID.Value);
                if (existingVariant != null)
                {
                    existingVariant.ColorID = variantDto.ColorID;
                    existingVariant.SizeID = variantDto.SizeID;
                    existingVariant.QuantityInStock = variantDto.QuantityInStock;
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