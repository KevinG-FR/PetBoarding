namespace PetBoarding_Application.Core.Reservations.GetUserDisplayedReservations;

using FluentResults;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Reservations;

internal sealed class GetUserDisplayedReservationsHandler : IQueryHandler<GetUserDisplayedReservationsQuery, IEnumerable<Reservation>>
{
    private readonly IReservationRepository _reservationRepository;

    public GetUserDisplayedReservationsHandler(IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<Result<IEnumerable<Reservation>>> Handle(
        GetUserDisplayedReservationsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Récupérer toutes les réservations de l'utilisateur
            var allUserReservations = await _reservationRepository.GetAllAsync(
                userId: request.UserId,
                serviceId: null,
                status: null,
                startDateMin: null,
                startDateMax: null,
                cancellationToken: cancellationToken);

            // Filtrer uniquement les statuts à afficher dans l'interface
            var displayedStatuses = new[] 
            {
                ReservationStatus.Validated,
                ReservationStatus.InProgress,
                ReservationStatus.Completed
            };

            var displayedReservations = allUserReservations
                .Where(r => displayedStatuses.Contains(r.Status))
                .ToList();

            return Result.Ok<IEnumerable<Reservation>>(displayedReservations);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error retrieving displayed reservations for user {request.UserId}: {ex.Message}");
        }
    }
}