namespace PetBoarding_Application.Baskets.ClearBasket;

using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Users;

internal sealed class ClearBasketCommandHandler : ICommandHandler<ClearBasketCommand>
{
    private readonly IBasketRepository _basketRepository;

    public ClearBasketCommandHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Result> Handle(ClearBasketCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        var basket = await _basketRepository.GetByUserIdAsync(userId, cancellationToken);
        if (basket is null)
            return Result.Fail("Basket not found");

        var clearResult = basket.ClearItems();
        if (clearResult.IsFailed)
            return clearResult;

        await _basketRepository.UpdateAsync(basket, cancellationToken);

        return Result.Ok();
    }
}