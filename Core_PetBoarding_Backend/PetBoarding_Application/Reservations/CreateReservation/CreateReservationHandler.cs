namespace PetBoarding_Application.Reservations.CreateReservation;

using FluentResults;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Reservations;

internal sealed class CreateReservationHandler : ICommandHandler<CreateReservationCommand, Reservation>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReservationHandler(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Reservation>> Handle(
        CreateReservationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var reservation = new Reservation(
                request.UserId,
                request.AnimalId,
                request.AnimalName,
                request.ServiceId,
                request.StartDate,
                request.EndDate,
                request.Comments);

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
