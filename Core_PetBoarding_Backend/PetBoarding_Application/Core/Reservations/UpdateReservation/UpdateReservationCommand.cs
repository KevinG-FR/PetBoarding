namespace PetBoarding_Application.Core.Reservations.UpdateReservation;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Reservations;

public sealed record UpdateReservationCommand(
    string Id,
    DateTime? StartDate,
    DateTime? EndDate,
    string? Comments) : ICommand<Reservation>;
