namespace PetBoarding_Application.Core.Reservations.GetReservationById;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Reservations;

public sealed record GetReservationByIdQuery(string Id) : IQuery<Reservation>;
