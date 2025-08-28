namespace PetBoarding_Api.Endpoints.Reservations;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetBoarding_Api.Dto.Reservations;
using PetBoarding_Api.Extensions;
using PetBoarding_Api.Mappers.Reservations;
using PetBoarding_Application.Reservations.ValidatePayment;

public static partial class ReservationsEndpoints
{
    private static async Task<IResult> ValidatePayment(
        [FromRoute] string id,
        [FromBody] ValidatePaymentRequest request,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ValidatePaymentCommand(
            id,
            request.AmountPaid,
            request.PaymentMethod ?? "BASKET_VALIDATION");

        var result = await mediator.Send(command, cancellationToken);

        return result.GetHttpResult(
            ReservationMapper.ToDto,
            reservation => reservation,
            reservation => $"/api/v1/reservations/{reservation.Id.Value}",
            SuccessStatusCode.Ok);
    }
}