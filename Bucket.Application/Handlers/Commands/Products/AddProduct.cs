using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Domain.Products;
using MediatR;

namespace Bucket.Application.Handlers.Commands.Products;

public record AddProductCommand(string Name, string Type, double Price) : IRequest<Result<long>>;

public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Result<long>>
{
    private readonly IProductRepository _productRepository;

    public AddProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<long>> Handle(AddProductCommand command, CancellationToken cancellationToken)
    {
        var productResult = Product.Create(command.Name, command.Type, command.Price);
        if (!productResult.IsSuccess)
        {
            return Result<long>.Failure(productResult.Error!);
        }

        var added = await _productRepository.AddProductAsync(productResult.Value!, cancellationToken);
        if (!added.IsSuccess)
        {
            return Result<long>.Failure(added.Error!);
        }

        return Result<long>.Success(added.Value);
    }
}
