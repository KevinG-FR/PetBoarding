namespace PetBoarding_Application.Reservations.CreateReservation;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Reservations;

public sealed record CreateReservationCommand(
    string UserId,
    string AnimalId,
    string AnimalName,
    string ServiceId,
    DateTime StartDate,
    DateTime? EndDate,
    string? Comments) : ICommand<Reservation>;
