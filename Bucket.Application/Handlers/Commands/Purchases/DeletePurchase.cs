using Bucket.Application.Repositories;
using Bucket.Common;
using MediatR;

namespace Bucket.Application.Handlers.Commands.Purchases;

public record DeletePurchaseCommand(long Id) : IRequest<Result>;

public class DeletePurchaseCommandHandler : IRequestHandler<DeletePurchaseCommand, Result>
{
    private readonly IPurchaseRepository _purchaseRepository;

    public DeletePurchaseCommandHandler(IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }

    public async Task<Result> Handle(DeletePurchaseCommand command, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(command.Id, cancellationToken);
        if (purchase is null)
        {
            return Result.NotFound("Purchase not found.");
        }

        var deleteResult = purchase.Delete();
        if (!deleteResult.IsSuccess)
        {
            return deleteResult;
        }

        var saved = await _purchaseRepository.UpdatePurchaseAsync(purchase, cancellationToken);
        if (!saved.IsSuccess)
        {
            return saved;
        }

        return Result.Ok();
    }
}
