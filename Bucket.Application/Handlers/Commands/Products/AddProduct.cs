using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Domain.Products;
using MediatR;

namespace Bucket.Application.Handlers.Commands.Products;

public record AddProductCommand(string Name, string Type, double Price) : IRequest<Result<EntityId>>;

public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Result<EntityId>>
{
    private readonly IProductRepository _productRepository;

    public AddProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<EntityId>> Handle(AddProductCommand command, CancellationToken cancellationToken)
    {
        var productResult = Product.Create(command.Name, command.Type, command.Price);
        if (!productResult.IsSuccess)
        {
            return Result<EntityId>.Failure(productResult.Error!);
        }

        var added = await _productRepository.AddProductAsync(productResult.Value!, cancellationToken);
        if (!added.IsSuccess)
        {
            return added.FailureKind == ResultFailureKind.NotFound
                ? Result<EntityId>.NotFound(added.Error!)
                : Result<EntityId>.Failure(added.Error!);
        }

        return Result<EntityId>.Success(new EntityId(added.Value));
    }
}
