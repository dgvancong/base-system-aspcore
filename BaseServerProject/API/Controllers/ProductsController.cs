using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaseServerProject.Application.Features.Products.Commands;
using BaseServerProject.Application.Features.Products.Queries;
using BaseServerProject.Application.Features.Products.DTOs;
using BaseServerProject.Application.Common.Base;

namespace BaseServerProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
        [FromQuery] string? searchTerm,
        [FromQuery] string? categoryName,
        [FromQuery] string? brandName,
        [FromQuery] string? status,
        [FromQuery] string? colors,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? sortBy,
        [FromQuery] bool isDescending = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetProductListQuery
        {
            SearchTerm = searchTerm,
            CategoryName = categoryName,
            BrandName = brandName,
            Status = status,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            SortBy = sortBy,
            Colors = colors,
            IsDescending = isDescending,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductRequestDto request)
    {
        var command = new CreateProductCommand
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
            Gender = request.Gender
        };
        var result = await _mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, new
        {
            success = true,
            message = "Product created successfully",
            data = result
        });
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var command = new DeleteProductCommand { ProductID = id };
        await _mediator.Send(command);
        return NoContent();
    }

}