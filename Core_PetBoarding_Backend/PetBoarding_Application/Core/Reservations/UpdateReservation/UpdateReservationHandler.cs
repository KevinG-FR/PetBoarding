namespace PetBoarding_Application.Core.Reservations.UpdateReservation;

using FluentResults;
using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Reservations;

internal sealed class UpdateReservationHandler : ICommandHandler<UpdateReservationCommand, Reservation>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReservationHandler(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Reservation>> Handle(
        UpdateReservationCommand request,
        CancellationToken cancellationToken)
    {
        // Valider l'ID de la réservation
        if (!Guid.TryParse(request.Id, out var guidId))
        {
            return Result.Fail("Invalid reservation ID format");
        }

        var reservationId = new ReservationId(guidId);
        
        // Récupérer la réservation
        var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);
        if (reservation is null)
        {
            return Result.Fail($"Reservation with ID {request.Id} not found");
        }

        try
        {
            // Mettre à jour les dates si fournies
            if (request.StartDate.HasValue)
            {
                reservation.UpdateDates(request.StartDate.Value, request.EndDate);
            }

            // Mettre à jour les commentaires si fournis (même si null pour effacer)
            if (request.Comments is not null)
            {
                reservation.UpdateComments(request.Comments);
            }

            // Sauvegarder les modifications
            var updatedReservation = await _reservationRepository.UpdateAsync(reservation, cancellationToken);
            if (updatedReservation is null)
            {
                return Result.Fail("Error occurred while updating the reservation");
            }


            return Result.Ok(updatedReservation);
        }        
        catch (Exception ex)
        {
            return Result.Fail($"Unexpected error updating reservation: {ex.Message}");
        }
    }   
}
