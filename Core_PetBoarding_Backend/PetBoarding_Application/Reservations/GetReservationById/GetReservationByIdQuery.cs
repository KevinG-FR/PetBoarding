namespace PetBoarding_Application.Reservations.GetReservationById;

using PetBoarding_Application.Abstractions;
using PetBoarding_Domain.Reservations;

public sealed record GetReservationByIdQuery(string Id) : IQuery<Reservation>;
