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
}
