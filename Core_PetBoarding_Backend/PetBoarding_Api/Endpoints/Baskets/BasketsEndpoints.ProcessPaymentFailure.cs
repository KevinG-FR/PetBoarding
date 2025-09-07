namespace PetBoarding_Api.Endpoints.Baskets;

using MediatR;
using PetBoarding_Application.Core.Payments.CreatePayment;
using PetBoarding_Application.Core.Payments.ProcessPayment;
using System.Security.Claims;

public static partial class BasketsEndpoints
{
    private static async Task<IResult> ProcessPaymentFailure(
        Guid basketId,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        // Créer un paiement
        var createPaymentCommand = new CreatePaymentCommand(
            userId,
            basketId,
            "stripe",
            "Mock payment for testing failure scenario");
        var createResult = await sender.Send(createPaymentCommand, cancellationToken);

        if (createResult.IsFailed)
        {
            return Results.BadRequest(createResult.Errors.Select(e => e.Message));
        }

        // Traiter le paiement comme un échec
        var processPaymentCommand = new ProcessPaymentCommand(
            createResult.Value.Id.Value,
            userId,
            IsSuccessful: false,
            ExternalTransactionId: null,
            FailureReason: "Payment simulation failure for testing purposes");

        var processResult = await sender.Send(processPaymentCommand, cancellationToken);

        if (processResult.IsFailed)
        {
            return Results.BadRequest(processResult.Errors.Select(e => e.Message));
        }

        return Results.Ok();
    }
}