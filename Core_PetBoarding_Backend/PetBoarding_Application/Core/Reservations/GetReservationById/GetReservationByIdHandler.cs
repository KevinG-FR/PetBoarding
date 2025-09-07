namespace PetBoarding_Application.Core.Reservations.GetReservationById;

using FluentResults;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Reservations;

internal sealed class GetReservationByIdHandler : IQueryHandler<GetReservationByIdQuery, Reservation>
{
    private readonly IReservationRepository _reservationRepository;

    public GetReservationByIdHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<Reservation>> Handle(
        GetReservationByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!Guid.TryParse(request.Id, out var guidId))
            {
                return Result.Fail("Invalid reservation ID");
            }

            var reservationId = new ReservationId(guidId);
            var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);

            if (reservation is null)
            {
                return Result.Fail("Reservation not found");
            }

            return Result.Ok(reservation);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving reservation: {ex.Message}");
        }
    }
}
