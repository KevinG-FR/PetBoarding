namespace PetBoarding_Application.Baskets.GetUserBasket;

using FluentResults;
using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Users;

internal sealed class GetUserBasketQueryHandler : IQueryHandler<GetUserBasketQuery, Basket?>
{
    private readonly IBasketRepository _basketRepository;

    public GetUserBasketQueryHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Result<Basket?>> Handle(GetUserBasketQuery request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        var basket = await _basketRepository.GetByUserIdWithItemsAsync(userId, cancellationToken);
        
        return Result.Ok(basket);
    }
}