namespace PetBoarding_Application.Reservations.CreateReservation;

using FluentResults;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Reservations;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;

internal sealed class CreateReservationHandler : ICommandHandler<CreateReservationCommand, Reservation>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IPlanningRepository _planningRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReservationHandler(
        IReservationRepository reservationRepository,
        IPlanningRepository planningRepository,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _planningRepository = planningRepository;
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
            var startDate = request.StartDate.Date;
            var endDate = request.EndDate?.Date ?? startDate;
            var currentDate = startDate;
            var slotsToReserve = new List<(DateTime Date, Guid SlotId)>();
            
            // Identifier tous les créneaux spécifiques à réserver
            while (currentDate <= endDate)
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
            var reservation = new Reservation(
                request.UserId,
                request.AnimalId,
                request.AnimalName,
                request.ServiceId,
                request.StartDate,
                request.EndDate,
                request.Comments);

            // 5. Ajouter les créneaux spécifiques à la réservation
            foreach (var (_, slotId) in reservedSlots)
            {
                reservation.AddReservedSlot(slotId);
            }

            // 6. Persister en base (transaction atomique)
            var createdReservation = await _reservationRepository.AddAsync(reservation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result.Ok(createdReservation);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error creating reservation: {ex.Message}");
        }
    }
}
