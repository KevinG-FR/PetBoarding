namespace PetBoarding_Api.Endpoints.Baskets;

using MediatR;
using PetBoarding_Application.Core.Baskets.ClearBasket;

public static partial class BasketsEndpoints
{
    private static async Task<IResult> ClearBasket(
        string basketId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(basketId, out var basketGuid))
        {
            return Results.BadRequest("Invalid basket ID format");
        }

        var command = new ClearBasketCommand(basketGuid);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return Results.BadRequest(result.Errors.Select(e => e.Message));
        }

        return Results.Ok();
    }
}