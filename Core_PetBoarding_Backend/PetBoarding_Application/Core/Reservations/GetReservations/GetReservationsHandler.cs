namespace PetBoarding_Application.Core.Reservations.GetReservations;

using FluentResults;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Reservations;

internal sealed class GetReservationsHandler : IQueryHandler<GetReservationsQuery, IEnumerable<Reservation>>
{
    private readonly IReservationRepository _reservationRepository;

    public GetReservationsHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<IEnumerable<Reservation>>> Handle(
        GetReservationsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var reservations = await _reservationRepository.GetAllAsync(
                userId: request.UserId,
                serviceId: request.ServiceId,
                status: request.Status,
                startDateMin: request.StartDateMin,
                startDateMax: request.StartDateMax,
                cancellationToken: cancellationToken);

            return Result.Ok(reservations);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving reservations: {ex.Message}");
        }
    }
}
