using MediatR;
using BaseServerProject.Application.Common.Interfaces;
using BaseServerProject.Application.Features.Products.Commands;

namespace BaseServerProject.Application.Features.Products.Handlers;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductID, cancellationToken);

        if (product == null)
        {
            return false;
        }

        await _productRepository.DeleteAsync(product, cancellationToken);

        return true;
    }

    public async Task<bool> Handles(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        return true;
    }
}