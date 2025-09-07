namespace PetBoarding_Application.Core.Reservations.GetUserDisplayedReservations;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Reservations;

public sealed record GetUserDisplayedReservationsQuery(
    string UserId) : IQuery<IEnumerable<Reservation>>;