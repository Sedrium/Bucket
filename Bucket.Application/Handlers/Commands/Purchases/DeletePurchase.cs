using Bucket.Application.Interfaces;
using Bucket.Common;
using MediatR;

namespace Bucket.Application.Handlers.Commands.Purchases;

public record DeletePurchaseCommand(long Id) : IRequest<Result<long>>;

public class DeletePurchaseCommandHandler : IRequestHandler<DeletePurchaseCommand, Result<long>>
{
    private readonly IPurchaseRepository _purchaseRepository;

    public DeletePurchaseCommandHandler(IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }

    public async Task<Result<long>> Handle(DeletePurchaseCommand command, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(command.Id, cancellationToken);
        if (purchase is null)
        {
            return Result<long>.NotFound("Purchase not found.");
        }

        var deleteResult = purchase.Delete();
        if (!deleteResult.IsSuccess)
        {
            return Result<long>.Failure(deleteResult.Error!);
        }

        var saved = await _purchaseRepository.UpdatePurchaseAsync(purchase, cancellationToken);
        if (!saved.IsSuccess)
        {
            return Result<long>.Failure(saved.Error!);
        }

        return Result<long>.Success(saved.Value);
    }
}
