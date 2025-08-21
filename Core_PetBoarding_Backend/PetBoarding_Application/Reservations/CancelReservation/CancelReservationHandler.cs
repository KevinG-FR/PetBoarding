namespace PetBoarding_Application.Reservations.CancelReservation;

using FluentResults;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Reservations;

internal sealed class CancelReservationHandler : ICommandHandler<CancelReservationCommand>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelReservationHandler(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        CancelReservationCommand request,
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

            reservation.Cancel();

            var updatedReservation = await _reservationRepository.UpdateAsync(reservation, cancellationToken);
            if (updatedReservation is null)
            {
                return Result.Fail("Error occurred while cancelling the reservation");
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Error cancelling reservation: {ex.Message}");
        }
    }
}
