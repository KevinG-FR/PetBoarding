namespace PetBoarding_Domain.Baskets;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Reservations;

public sealed class BasketItem : Entity<BasketItemId>
{
    public BasketItem(
        BasketId basketId,
        ReservationId reservationId) : base(new BasketItemId(Guid.CreateVersion7()))
    {
        BasketId = basketId;
        ReservationId = reservationId;
        AddedAt = DateTime.UtcNow;
    }

    public BasketId BasketId { get; private set; }
    public ReservationId ReservationId { get; private set; }
    public Reservation? Reservation { get; private set; }
    public DateTime AddedAt { get; private set; }

    public decimal GetTotalPrice()
    {
        return Reservation?.TotalPrice ?? 0;
    }
}