using MediatR;
using Microsoft.EntityFrameworkCore;
using BaseServerProject.Application.Common.Interfaces;
using BaseServerProject.Application.Common.Base;
using BaseServerProject.Application.Features.Products.DTOs;
using BaseServerProject.Application.Features.Products.Queries;

namespace BaseServerProject.Application.Features.Products.Handlers;

public class GetProductListQueryHandler : IRequestHandler<GetProductListQuery, PagedResult<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductListQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductDto>> Handle(GetProductListQuery request, CancellationToken cancellationToken)
    {
        var query = _productRepository.GetQueryable();

        // Filters
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(p =>
                p.ProductCode.Contains(request.SearchTerm) ||
                p.ProductName.Contains(request.SearchTerm) ||
                (p.BrandName != null && p.BrandName.Contains(request.SearchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(request.CategoryName))
        {
            query = query.Where(p => p.CategoryName == request.CategoryName);
        }

        if (!string.IsNullOrWhiteSpace(request.BrandName))
        {
            query = query.Where(p => p.BrandName == request.BrandName);
        }
        
        if (!string.IsNullOrWhiteSpace(request.Colors))
        {
            var colorList = request.Colors.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                          .Select(c => c.Trim())
                                          .ToList();

            query = query.Where(p => p.Color != null &&
                colorList.Any(c => p.Color.Contains(c)));
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(p => p.Status == request.Status);
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.SellingPrice >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.SellingPrice <= request.MaxPrice.Value);
        }

        // Sorting
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.IsDescending
                ? query.OrderByDescending(p => p.ProductName)
                : query.OrderBy(p => p.ProductName),
            "price" => request.IsDescending
                ? query.OrderByDescending(p => p.SellingPrice)
                : query.OrderBy(p => p.SellingPrice),
            "stock" => request.IsDescending
                ? query.OrderByDescending(p => p.QuantityInStock)
                : query.OrderBy(p => p.QuantityInStock),
            "code" => request.IsDescending
                ? query.OrderByDescending(p => p.ProductCode)
                : query.OrderBy(p => p.ProductCode),
            _ => query.OrderByDescending(p => p.ProductID)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Map thủ công
        var itemDtos = items.Select(p => new ProductDto
        {
            ProductID = p.ProductID,
            ProductCode = p.ProductCode,
            ProductName = p.ProductName,
            SupplierName = p.SupplierName,
            CategoryName = p.CategoryName,
            Color = p.Color,
            Size = p.Size,
            PurchasePrice = p.PurchasePrice,
            SellingPrice = p.SellingPrice,
            QuantityInStock = p.QuantityInStock,
            Description = p.Description,
            BrandName = p.BrandName,
            Material = p.Material,
            Gender = p.Gender,
            Status = p.Status,
            CreatedDate = p.CreatedDate,
            UpdatedDate = p.UpdatedDate
        }).ToList();

        return new PagedResult<ProductDto>
        {
            Items = itemDtos,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}