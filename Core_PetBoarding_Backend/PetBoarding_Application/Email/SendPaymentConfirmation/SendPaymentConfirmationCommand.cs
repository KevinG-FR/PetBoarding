using PetBoarding_Application.Abstractions;

namespace PetBoarding_Application.Email.SendPaymentConfirmation;

public sealed record SendPaymentConfirmationCommand(
    string Email,
    string CustomerName,
    string PaymentId,
    decimal Amount,
    DateTime PaymentDate,
    string PaymentMethod,
    string Status,
    string? ReservationNumber = null,
    string? ServiceName = null
) : ICommand;