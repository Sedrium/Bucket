using Bucket.Application.Interfaces;
using Bucket.Common;
using Bucket.Domain.Purchases;
using MediatR;

namespace Bucket.Application.Handlers.Commands.Purchases;

public record AddPurchaseCommand(long CustomerId, IReadOnlyList<long> ProductIds) : IRequest<Result<long>>;

public class AddPurchaseCommandHandler : IRequestHandler<AddPurchaseCommand, Result<long>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPurchaseRepository _purchaseRepository;

    public AddPurchaseCommandHandler(
        IPersonRepository personRepository,
        IProductRepository productRepository,
        IPurchaseRepository purchaseRepository)
    {
        _personRepository = personRepository;
        _productRepository = productRepository;
        _purchaseRepository = purchaseRepository;
    }

    public async Task<Result<long>> Handle(AddPurchaseCommand command, CancellationToken cancellationToken)
    {
        var customer = await _personRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer is null)
        {
            return Result<long>.Failure("Customer not found.");
        }

        foreach (var productId in command.ProductIds)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product is null)
            {
                return Result<long>.Failure($"Product with id {productId} not found.");
            }
        }

        var purchaseResult = Purchase.Create(command.CustomerId, command.ProductIds);
        if (!purchaseResult.IsSuccess)
        {
            return Result<long>.Failure(purchaseResult.Error!);
        }

        var added = await _purchaseRepository.AddPurchaseAsync(purchaseResult.Value!, cancellationToken);
        if (!added.IsSuccess)
        {
            return Result<long>.Failure(added.Error!);
        }

        return Result<long>.Success(added.Value);
    }
}
