namespace PetBoarding_Application.Reservations.UpdateReservation;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Reservations;

public sealed record UpdateReservationCommand(
    string Id,
    DateTime? StartDate,
    DateTime? EndDate,
    string? Comments) : ICommand<Reservation>;
