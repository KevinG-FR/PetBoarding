namespace PetBoarding_Application.Core.Reservations.ValidatePayment;

using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Reservations;

/// <summary>
/// Commande pour valider le paiement d'une réservation
/// </summary>
public sealed record ValidatePaymentCommand(
    string ReservationId,
    decimal AmountPaid,
    string PaymentMethod = "BASKET_VALIDATION" // Pour le moment, paiement via panier
) : ICommand<Reservation>;