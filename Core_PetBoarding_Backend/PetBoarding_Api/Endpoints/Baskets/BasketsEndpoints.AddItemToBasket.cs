namespace PetBoarding_Api.Endpoints.Baskets;

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Application.Baskets.AddItemToBasket;
using PetBoarding_Api.Dto.Baskets;

public static partial class BasketsEndpoints
{
    private static async Task<IResult> AddItemToBasket(
        [FromBody] AddItemToBasketRequest request,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        var command = new AddItemToBasketCommand(userId, request.ReservationId);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return Results.BadRequest(result.Errors.Select(e => e.Message));
        }

        return Results.Ok();
    }
}