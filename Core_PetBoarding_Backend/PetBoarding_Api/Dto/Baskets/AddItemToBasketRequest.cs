namespace PetBoarding_Api.Dto.Baskets;

public sealed record AddItemToBasketRequest(
    Guid ReservationId
);