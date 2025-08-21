namespace PetBoarding_Application.Reservations.GetReservations;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Reservations;

public sealed record GetReservationsQuery(
    string? UserId = null,
    string? ServiceId = null,
    ReservationStatus? Status = null,
    DateTime? StartDateMin = null,
    DateTime? StartDateMax = null) : IQuery<IEnumerable<Reservation>>;
