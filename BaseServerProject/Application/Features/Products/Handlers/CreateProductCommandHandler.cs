using MediatR;
using BaseServerProject.Application.Common.Interfaces;
using BaseServerProject.Application.Features.Products.Commands;
using BaseServerProject.Application.Features.Products.DTOs;
using BaseServerProject.Core.Entities;

namespace BaseServerProject.Application.Features.Products.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public CreateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Check duplicate
        var existingProduct = await _productRepository.GetByCodeAsync(request.ProductCode, cancellationToken);
        if (existingProduct != null)
        {
            throw new InvalidOperationException($"Product code '{request.ProductCode}' already exists");
        }

        var product = new Product
        {
            ProductCode = request.ProductCode,
            ProductName = request.ProductName,
            SupplierName = request.SupplierName,
            CategoryName = request.CategoryName,
            Color = request.Color,
            Size = request.Size,
            PurchasePrice = request.PurchasePrice,
            SellingPrice = request.SellingPrice,
            QuantityInStock = request.QuantityInStock,
            Description = request.Description,
            BrandName = request.BrandName,
            Material = request.Material,
            Gender = request.Gender,
            Status = request.QuantityInStock > 0 ? "Đang bán" : "Hết hàng",
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        var createdProduct = await _productRepository.CreateAsync(product, cancellationToken);

        // Map thủ công
        return new ProductDto
        {
            ProductID = createdProduct.ProductID,
            ProductCode = createdProduct.ProductCode,
            ProductName = createdProduct.ProductName,
            SupplierName = createdProduct.SupplierName,
            CategoryName = createdProduct.CategoryName,
            Color = createdProduct.Color,
            Size = createdProduct.Size,
            PurchasePrice = createdProduct.PurchasePrice,
            SellingPrice = createdProduct.SellingPrice,
            QuantityInStock = createdProduct.QuantityInStock,
            Description = createdProduct.Description,
            BrandName = createdProduct.BrandName,
            Material = createdProduct.Material,
            Gender = createdProduct.Gender,
            Status = createdProduct.Status,
            CreatedDate = createdProduct.CreatedDate,
            UpdatedDate = createdProduct.UpdatedDate
        };
    }
}