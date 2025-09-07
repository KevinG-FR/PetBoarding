namespace PetBoarding_Api.Endpoints.Baskets;

using MediatR;
using PetBoarding_Application.Core.Baskets.RemoveItemFromBasket;
using System.Security.Claims;

public static partial class BasketsEndpoints
{
    private static async Task<IResult> RemoveItemFromBasket(
        Guid basketItemId,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        var command = new RemoveItemFromBasketCommand(userId, basketItemId);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return Results.BadRequest(result.Errors.Select(e => e.Message));
        }

        return Results.Ok();
    }
}