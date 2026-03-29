using Bucket.Application.Interfaces;
using Bucket.Common;
using MediatR;

namespace Bucket.Application.Handlers.Commands.Products;

public record DeleteProductCommand(long Id) : IRequest<Result>;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(command.Id, cancellationToken);
        if (product is null)
        {
            return Result.NotFound("Product not found.");
        }

        var deleteResult = product.Delete();
        if (!deleteResult.IsSuccess)
        {
            return deleteResult;
        }

        var saved = await _productRepository.UpdateProductAsync(product, cancellationToken);
        if (!saved.IsSuccess)
        {
            return saved;
        }

        return Result.Ok();
    }
}
