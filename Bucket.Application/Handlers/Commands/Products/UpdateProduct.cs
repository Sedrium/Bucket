using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Domain.Products;
using MediatR;

namespace Bucket.Application.Handlers.Commands.Products;

public record UpdateProductCommand(long Id, string Name, string Type, double Price) : IRequest<Result<long>>;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<long>>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<long>> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var existing = await _productRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existing is null)
        {
            return Result<long>.Failure("Product not found.");
        }

        var updateResult = existing.Update(command.Name, command.Type, command.Price);
        if (!updateResult.IsSuccess)
        {
            return Result<long>.Failure(updateResult.Error!);
        }

        var saved = await _productRepository.UpdateProductAsync(existing, cancellationToken);
        if (!saved.IsSuccess)
        {
            return Result<long>.Failure(saved.Error!);
        }

        return Result<long>.Success(saved.Value);
    }
}
