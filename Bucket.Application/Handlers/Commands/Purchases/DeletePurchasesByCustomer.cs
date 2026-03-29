using Bucket.Application.Interfaces;
using Bucket.Common;
using MediatR;

namespace Bucket.Application.Handlers.Commands.Purchases;

public record DeletePurchasesByCustomerCommand(long CustomerId) : IRequest<Result>;

public class DeletePurchasesByCustomerCommandHandler : IRequestHandler<DeletePurchasesByCustomerCommand, Result>
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

    public async Task<Result> Handle(DeletePurchasesByCustomerCommand command, CancellationToken cancellationToken)
    {
        var customer = await _personRepository.GetByIdAsync(command.CustomerId, cancellationToken);
        if (customer is null)
        {
            return Result.NotFound("Customer not found.");
        }

        var purchases = await _purchaseRepository.GetActiveByCustomerIdAsync(command.CustomerId, cancellationToken);
        if (purchases.Count == 0)
        {
            return Result.NotFound("No active purchases for this customer.");
        }

        foreach (var purchase in purchases)
        {
            var deleteResult = purchase.Delete();
            if (!deleteResult.IsSuccess)
            {
                return Result.Fail(deleteResult.Error!);
            }

            var saved = await _purchaseRepository.UpdatePurchaseAsync(purchase, cancellationToken);
            if (!saved.IsSuccess)
            {
                return saved;
            }
        }

        return Result.Ok();
    }
}
