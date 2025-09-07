using PetBoarding_Application.Core.Abstractions;

namespace PetBoarding_Application.Core.Email.SendPaymentConfirmation;

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