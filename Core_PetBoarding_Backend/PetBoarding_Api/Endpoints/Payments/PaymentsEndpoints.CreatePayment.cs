namespace PetBoarding_Api.Endpoints.Payments;

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Application.Core.Payments.CreatePayment;
using PetBoarding_Api.Dto.Payments;

public static partial class PaymentsEndpoints
{
    private static async Task<IResult> CreatePayment(
        [FromBody] CreatePaymentRequest request,
        ClaimsPrincipal user,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        var command = new CreatePaymentCommand(userId, request.BasketId, request.Method, request.Description);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return Results.BadRequest(result.Errors.Select(e => e.Message));
        }

        var payment = result.Value;
        var paymentResponse = new PaymentResponse(
            payment.Id.Value,
            payment.Amount,
            payment.Method.Name,
            payment.Status.Name,
            payment.ExternalTransactionId,
            payment.Description,
            payment.ProcessedAt,
            payment.FailureReason,
            payment.CreatedAt,
            payment.UpdatedAt
        );

        return Results.Created($"/api/payments/{paymentResponse.Id}", paymentResponse);
    }
}