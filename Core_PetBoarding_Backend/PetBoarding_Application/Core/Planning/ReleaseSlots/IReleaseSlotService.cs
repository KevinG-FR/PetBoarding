namespace PetBoarding_Application.Core.Planning.ReleaseSlots;

using FluentResults;
using PetBoarding_Domain.Reservations;

/// <summary>
/// Service pour la libération des créneaux de réservation
/// </summary>
public interface IReleaseSlotService
{
    /// <summary>
    /// Libère tous les créneaux actifs d'une réservation
    /// </summary>
    /// <param name="reservationId">ID de la réservation</param>
    /// <param name="cancellationToken">Token d'annulation</param>
    /// <returns>Résultat de l'opération avec le nombre de créneaux libérés</returns>
    Task<Result<int>> ReleaseReservationSlotsAsync(
        ReservationId reservationId, 
        CancellationToken cancellationToken = default);
}