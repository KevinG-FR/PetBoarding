namespace PetBoarding_Application.Core.Baskets.UpdateBasketItem;

using FluentResults;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Prestations;
using PetBoarding_Domain.Users;

internal sealed class UpdateBasketItemCommandHandler : ICommandHandler<UpdateBasketItemCommand>
{
    private readonly IBasketRepository _basketRepository;

    public UpdateBasketItemCommandHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Result> Handle(UpdateBasketItemCommand request, CancellationToken cancellationToken)
    {
        // This operation is no longer supported since we removed quantity concept
        // Each reservation in the basket is unique and cannot have its quantity updated
        return Result.Fail("Update basket item quantity is no longer supported. Each reservation is unique.");
    }
}