namespace PetBoarding_Domain.Reservations;

/// <summary>
/// Statuts de réservation avec gestion temporelle des paiements
/// </summary>
public enum ReservationStatus
{
    /// <summary>
    /// Réservation créée, créneaux temporairement réservés (max 20 min)
    /// </summary>
    Created = 1,
    
    /// <summary>
    /// Réservation payée et validée (créneaux définitivement réservés)
    /// </summary>
    Validated = 2,
    
    /// <summary>
    /// Annulation automatique après expiration des 20 minutes (créneaux libérés)
    /// </summary>
    CancelAuto = 3,
    
    /// <summary>
    /// Annulation manuelle par le client (créneaux libérés)
    /// </summary>
    Cancelled = 4,
    
    /// <summary>
    /// Service en cours de réalisation
    /// </summary>
    InProgress = 5,
    
    /// <summary>
    /// Service terminé
    /// </summary>
    Completed = 6,
}
