namespace PetBoarding_Domain.Baskets;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Users;

public interface IBasketRepository : IBaseRepository<Basket, BasketId>
{
    Task<Basket?> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<Basket?> GetByUserIdWithItemsAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<Basket?> GetActiveBasketByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<Basket?> GetActiveBasketByUserIdWithItemsAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<Basket?> GetByIdWithItemsAsync(BasketId basketId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Basket>> GetExpiredBaskets(CancellationToken cancellationToken = default);
    Task<IEnumerable<Basket>> GetExpiredBaskets(int expirationMinutes, CancellationToken cancellationToken = default);
    Task<IEnumerable<Basket>> GetBasketsWithPaymentFailures(CancellationToken cancellationToken = default);
}