using PetBoarding_Application.Abstractions;

namespace PetBoarding_Application.Email.SendReservationConfirmation;

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