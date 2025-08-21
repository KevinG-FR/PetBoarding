namespace PetBoarding_Application.Reservations.CancelReservation;

using PetBoarding_Application.Abstractions;

public sealed record CancelReservationCommand(string Id) : ICommand;
