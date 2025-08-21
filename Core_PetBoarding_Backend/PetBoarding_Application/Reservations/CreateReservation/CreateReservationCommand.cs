namespace PetBoarding_Application.Reservations.CreateReservation;

using PetBoarding_Application.Abstractions;

public sealed record CreateReservationCommand(
    string UserId,
    string AnimalId,
    string AnimalName,
    string ServiceId,
    DateTime StartDate,
    DateTime? EndDate,
    string? Comments) : ICommand<string>;
