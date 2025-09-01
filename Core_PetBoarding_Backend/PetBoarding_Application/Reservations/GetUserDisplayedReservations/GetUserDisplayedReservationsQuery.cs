namespace PetBoarding_Application.Reservations.GetUserDisplayedReservations;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Reservations;

public sealed record GetUserDisplayedReservationsQuery(
    string UserId) : IQuery<IEnumerable<Reservation>>;