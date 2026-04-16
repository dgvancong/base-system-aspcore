using MediatR;

namespace BaseServerProject.Application.Features.Products.Commands;

public class DeleteProductCommand : IRequest<bool>
{
    public int ProductID { get; set; } 
}