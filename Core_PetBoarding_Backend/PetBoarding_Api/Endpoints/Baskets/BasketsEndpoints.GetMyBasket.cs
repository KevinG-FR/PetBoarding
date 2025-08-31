namespace PetBoarding_Api.Endpoints.Baskets;

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Application.Baskets.GetUserBasket;
using PetBoarding_Api.Dto.Baskets;

public static partial class BasketsEndpoints
{
    private static async Task<IResult> GetMyBasket(
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        var query = new GetUserBasketQuery(userId);
        var result = await sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return Results.BadRequest(result.Errors.Select(e => e.Message));
        }

        if (result.Value is null)
        {
            return Results.NoContent();
        }

        var basket = result.Value;
        var basketResponse = new BasketResponse(
            basket.Id.Value,
            basket.UserId.Value,
            basket.Status.Name,
            basket.GetTotalAmount(),
            basket.GetTotalItemCount(),
            basket.PaymentFailureCount,
            basket.PaymentId?.Value,
            basket.CreatedAt,
            basket.UpdatedAt,
            basket.Items.Select(item => new BasketItemResponse(
                item.Id.Value,
                item.ReservationId.Value,
                $"Reservation for {item.Reservation?.AnimalName ?? "Unknown"}",
                item.Reservation?.TotalPrice ?? 0,
                item.AddedAt
            ))
        );

        return Results.Ok(basketResponse);
    }
}