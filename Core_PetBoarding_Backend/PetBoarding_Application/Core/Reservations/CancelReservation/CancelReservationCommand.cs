namespace PetBoarding_Application.Core.Reservations.CancelReservation;

using PetBoarding_Application.Core.Abstractions;

public sealed record CancelReservationCommand(string Id) : ICommand;
