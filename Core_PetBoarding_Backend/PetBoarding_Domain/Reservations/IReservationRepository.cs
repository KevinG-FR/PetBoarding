namespace PetBoarding_Domain.Reservations;

using PetBoarding_Domain.Abstractions;

public interface IReservationRepository : IBaseRepository<Reservation, ReservationId>
{
    Task<IEnumerable<Reservation>> GetAllAsync(
        string? userId = null,
        string? serviceId = null,
        ReservationStatus? status = null,
        DateTime? startDateMin = null,
        DateTime? startDateMax = null,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Reservation>> GetByUserIdAsync(
        string userId, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Reservation>> GetByServiceIdAsync(
        string serviceId, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Reservation>> GetReservationsBetweenDatesAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
        
    /// <summary>
    /// Récupère toutes les réservations avec statut Created qui ont expiré (PaymentExpiryAt dépassé)
    /// </summary>
    Task<IEnumerable<Reservation>> GetExpiredCreatedReservationsAsync(
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Récupère les réservations d'un utilisateur avec les statuts affichables dans l'interface (Validated, InProgress, Completed)
    /// </summary>
    Task<IEnumerable<Reservation>> GetUserDisplayedReservationsAsync(
        string userId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Récupère les réservations pour un service spécifique dans une plage de dates
    /// </summary>
    Task<IEnumerable<Reservation>> GetByServiceIdAndDateRangeAsync(
        string serviceId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}
