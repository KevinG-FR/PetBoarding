namespace PetBoarding_Api.Endpoints.Baskets;

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Application.Payments.CreatePayment;
using PetBoarding_Application.Payments.ProcessPayment;
using PetBoarding_Domain.Payments;

public static partial class BasketsEndpoints
{
    private static async Task<IResult> ProcessPaymentSuccess(
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
            "Mock payment for testing success scenario");
        var createResult = await sender.Send(createPaymentCommand, cancellationToken);

        if (createResult.IsFailed)
        {
            return Results.BadRequest(createResult.Errors.Select(e => e.Message));
        }

        // Traiter le paiement comme un succès
        var processPaymentCommand = new ProcessPaymentCommand(
            createResult.Value.Id.Value,
            userId,
            IsSuccessful: true,
            ExternalTransactionId: Guid.NewGuid().ToString(),
            FailureReason: null);

        var processResult = await sender.Send(processPaymentCommand, cancellationToken);

        if (processResult.IsFailed)
        {
            return Results.BadRequest(processResult.Errors.Select(e => e.Message));
        }

        return Results.Ok();
    }
}