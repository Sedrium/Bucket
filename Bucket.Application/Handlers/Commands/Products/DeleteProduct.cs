using Bucket.Application.Interfaces;
using Bucket.Common;
using MediatR;

namespace Bucket.Application.Handlers.Commands.Products;

public record DeleteProductCommand(long Id) : IRequest<Result<long>>;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<long>>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<long>> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(command.Id, cancellationToken);
        if (product is null)
        {
            return Result<long>.NotFound("Product not found.");
        }

        var deleteResult = product.Delete();
        if (!deleteResult.IsSuccess)
        {
            return Result<long>.Failure(deleteResult.Error!);
        }

        var saved = await _productRepository.UpdateProductAsync(product, cancellationToken);
        if (!saved.IsSuccess)
        {
            return Result<long>.Failure(saved.Error!);
        }

        return Result<long>.Success(saved.Value);
    }
}
