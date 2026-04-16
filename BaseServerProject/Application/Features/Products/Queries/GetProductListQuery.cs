using MediatR;
using BaseServerProject.Application.Common.Base;
using BaseServerProject.Application.Features.Products.DTOs;

namespace BaseServerProject.Application.Features.Products.Queries;

public class GetProductListQuery : IRequest<PagedResult<ProductDto>>
{
    public string? SearchTerm { get; set; }
    public string? CategoryName { get; set; }
    public string? BrandName { get; set; }
    public string? Status { get; set; }
    public string? Colors { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}