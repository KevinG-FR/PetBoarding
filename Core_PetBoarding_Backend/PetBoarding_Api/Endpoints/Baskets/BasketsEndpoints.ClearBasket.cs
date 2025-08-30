namespace PetBoarding_Api.Endpoints.Baskets;

using System.Security.Claims;
using MediatR;
using PetBoarding_Application.Baskets.ClearBasket;

public static partial class BasketsEndpoints
{
    private static async Task<IResult> ClearBasket(
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        var command = new ClearBasketCommand(userId);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return Results.BadRequest(result.Errors.Select(e => e.Message));
        }

        return Results.Ok();
    }
}