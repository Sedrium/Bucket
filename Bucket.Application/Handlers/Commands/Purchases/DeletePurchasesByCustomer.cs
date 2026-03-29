using Bucket.Application.Interfaces;
using Bucket.Common;
using MediatR;

namespace Bucket.Application.Handlers.Commands.Purchases;

public record DeletePurchasesByCustomerCommand(long CustomerId) : IRequest<Result<int>>;

public class DeletePurchasesByCustomerCommandHandler : IRequestHandler<DeletePurchasesByCustomerCommand, Result<int>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IPurchaseRepository _purchaseRepository;

    public DeletePurchasesByCustomerCommandHandler(
        IPersonRepository personRepository,
        IPurchaseRepository purchaseRepository)
    {
        _personRepository = personRepository;
        _purchaseRepository = purchaseRepository;
    }

    public async Task<Result<int>> Handle(DeletePurchasesByCustomerCommand command, CancellationToken cancellationToken)
    {
        var customer = await _personRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer is null)
        {
            return Result<int>.Failure("Customer not found.");
        }

        var purchases = await _purchaseRepository.GetActiveByCustomerIdAsync(command.CustomerId, cancellationToken);
        if (purchases.Count == 0)
        {
            return Result<int>.Failure("No active purchases for this customer.");
        }

        var count = 0;
        foreach (var purchase in purchases)
        {
            var deleteResult = purchase.Delete();
            if (!deleteResult.IsSuccess)
            {
                return Result<int>.Failure(deleteResult.Error!);
            }

            var saved = await _purchaseRepository.UpdatePurchaseAsync(purchase, cancellationToken);
            if (!saved.IsSuccess)
            {
                return Result<int>.Failure(saved.Error!);
            }

            count++;
        }

        return Result<int>.Success(count);
    }
}
