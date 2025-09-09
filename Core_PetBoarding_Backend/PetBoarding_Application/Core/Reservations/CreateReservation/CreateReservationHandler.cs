namespace PetBoarding_Application.Core.Reservations.CreateReservation;

using FluentResults;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Reservations;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;

internal sealed class CreateReservationHandler : ICommandHandler<CreateReservationCommand, Reservation>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IPlanningRepository _planningRepository;
    private readonly IPrestationRepository _prestationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReservationHandler(
        IReservationRepository reservationRepository,
        IPlanningRepository planningRepository,
        IPrestationRepository prestationRepository,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _planningRepository = planningRepository;
        _prestationRepository = prestationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Reservation>> Handle(
        CreateReservationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Valider le ServiceId et obtenir le planning
            if (!Guid.TryParse(request.ServiceId, out var serviceGuid))
            {
                return Result.Fail("Invalid ServiceId format");
            }

            var prestationId = new PrestationId(serviceGuid);
            var planning = await _planningRepository.GetByPrestationIdAsync(prestationId, cancellationToken);
            
            if (planning is null)
            {
                return Result.Fail("No planning found for this service");
            }

            // 2. Récupérer et vérifier la disponibilité des créneaux spécifiques
            var startDate = ParseDateSafely(request.StartDate);
            var endDateForSlots = request.EndDate.HasValue ? ParseDateSafely(request.EndDate.Value) : startDate;
            var currentDate = startDate;
            var slotsToReserve = new List<(DateTime Date, Guid SlotId)>();
            
            // Identifier tous les créneaux spécifiques à réserver
            while (currentDate <= endDateForSlots)
            {
                var availableSlot = planning.GetSlotForDate(currentDate);
                if (availableSlot is null || !availableSlot.IsAvailable(1))
                {
                    return Result.Fail($"No available slot for date {currentDate:yyyy-MM-dd}");
                }
                
                slotsToReserve.Add((currentDate, availableSlot.Id.Value));
                currentDate = currentDate.AddDays(1);
            }

            // 3. Réserver temporairement tous les créneaux (20 minutes max)
            var reservedSlots = new List<(DateTime Date, Guid SlotId)>();
            
            try
            {
                foreach (var (date, slotId) in slotsToReserve)
                {
                    planning.ReserveSlot(date, 1);
                    reservedSlots.Add((date, slotId));
                }
            }
            catch (InvalidOperationException ex)
            {
                // En cas d'échec, libérer les créneaux déjà réservés
                foreach (var (date, _) in reservedSlots)
                {
                    try { planning.CancelReservation(date, 1); } catch { /* Ignore errors during rollback */ }
                }
                return Result.Fail($"Failed to reserve slots: {ex.Message}");
            }

            // 4. Créer la réservation avec statut Created
            var reservation = Reservation.Create(
                request.UserId,
                request.AnimalId,
                request.AnimalName,
                request.ServiceId,
                startDate,
                request.EndDate.HasValue ? ParseDateSafely(request.EndDate.Value) : null,
                request.Comments);

            // 5. Ajouter les créneaux spécifiques à la réservation
            foreach (var (_, slotId) in reservedSlots)
            {
                reservation.AddReservedSlot(slotId);
            }

            // 6. Calculer et définir le prix total de la réservation
            var prestation = await _prestationRepository.GetByIdAsync(prestationId, cancellationToken);
            if (prestation is null)
            {
                // En cas d'échec, libérer les créneaux réservés
                foreach (var (date, _) in reservedSlots)
                {
                    try { planning.CancelReservation(date, 1); } catch { /* Ignore errors during rollback */ }
                }
                return Result.Fail("Prestation not found for calculating reservation price");
            }

            var numberOfDays = reservation.GetNumberOfDays();
            var totalPrice = prestation.Prix * numberOfDays;
            reservation.SetTotalPrice(totalPrice);

            // 7. Persister en base (transaction atomique)
            var createdReservation = await _reservationRepository.AddAsync(reservation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Ok(createdReservation);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error creating reservation: {ex.Message}");
        }
    }

    /// <summary>
    /// Parse une date de façon sécurisée en évitant les problèmes de timezone
    /// </summary>
    private static DateTime ParseDateSafely(DateTime dateTime)
    {
        // Si la date a été parsée par ASP.NET Core avec une timezone,
        // on force l'extraction de la partie date sans conversion timezone
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, DateTimeKind.Unspecified);
    }
}
