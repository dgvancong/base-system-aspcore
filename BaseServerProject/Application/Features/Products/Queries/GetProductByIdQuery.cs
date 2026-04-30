using MediatR;
using BaseServerProject.Application.Features.Products.DTOs;

namespace BaseServerProject.Application.Features.Products.Queries;

public class GetProductByIdQuery : IRequest<ProductDto>
{
    public int ProductID { get; set; }
}