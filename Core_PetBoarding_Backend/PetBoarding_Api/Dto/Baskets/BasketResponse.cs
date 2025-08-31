namespace PetBoarding_Api.Dto.Baskets;

public sealed record BasketResponse(
    Guid Id,
    Guid UserId,
    string Status,
    decimal TotalAmount,
    int TotalItemCount,
    int PaymentFailureCount,
    Guid? PaymentId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IEnumerable<BasketItemResponse> Items
);

public sealed record BasketItemResponse(
    Guid Id,
    Guid ReservationId,
    string ServiceName,
    decimal ReservationPrice,
    string ReservationDates
);