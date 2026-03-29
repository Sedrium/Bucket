using Bucket.Application.Repositories;
using Bucket.Common;
using Bucket.Domain.Purchases;
using MediatR;

namespace Bucket.Application.Handlers.Commands.Purchases;

public record AddPurchaseCommand(long CustomerId, IReadOnlyList<long> ProductIds) : IRequest<Result<EntityId>>;

public class AddPurchaseCommandHandler : IRequestHandler<AddPurchaseCommand, Result<EntityId>>
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

    public async Task<Result<EntityId>> Handle(AddPurchaseCommand command, CancellationToken cancellationToken)
    {
        var customer = await _personRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer is null)
        {
            return Result<EntityId>.NotFound("Customer not found.");
        }

        var distinctProductIds = command.ProductIds.Distinct().ToList();
        var products = await _productRepository.GetByIdsAsync(distinctProductIds, cancellationToken);
        if (products.Count != distinctProductIds.Count)
        {
            return Result<EntityId>.NotFound("One or more products not found.");
        }

        var purchase = Purchase.Create(command.CustomerId, [.. products.Select(p => p.Id!.Value)]);
        if (!purchase.IsSuccess)
        {
            return Result<EntityId>.Failure(purchase.Error!);
        }

        var added = await _purchaseRepository.AddPurchaseAsync(purchase.Value!, cancellationToken);

        return Result<EntityId>.Success(new EntityId(added.Value));
    }
}
