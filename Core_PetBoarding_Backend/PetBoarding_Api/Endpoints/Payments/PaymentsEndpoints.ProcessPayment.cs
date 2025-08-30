namespace PetBoarding_Api.Endpoints.Payments;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Application.Payments.ProcessPayment;

public static partial class PaymentsEndpoints
{
    public sealed record ProcessPaymentRequest(
        bool IsSuccessful,
        string? ExternalTransactionId = null,
        string? FailureReason = null
    );

    private static async Task<IResult> ProcessPayment(
        Guid paymentId,
        [FromBody] ProcessPaymentRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ProcessPaymentCommand(
            paymentId,
            request.IsSuccessful,
            request.ExternalTransactionId,
            request.FailureReason);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return Results.BadRequest(result.Errors.Select(e => e.Message));
        }

        return Results.Ok();
    }
}