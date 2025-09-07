using PetBoarding_Application.Core.Abstractions;

namespace PetBoarding_Application.Core.Email.SendReservationConfirmation;

public sealed record SendReservationConfirmationCommand(
    string Email,
    string CustomerName,
    string PetName,
    string ServiceName,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalAmount,
    string ReservationNumber,
    List<string> SpecialInstructions
) : ICommand;