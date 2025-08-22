namespace PetBoarding_Domain.Reservations;

using PetBoarding_Domain.Abstractions;

/// <summary>
/// Entity representing a reservation with schedule management
/// </summary>
public sealed class Reservation : Entity<ReservationId>
{
    public Reservation(
        string userId,
        string animalId,
        string animalName,
        string serviceId,
        DateTime startDate,
        DateTime? endDate = null,
        string? comments = null) : base(new ReservationId(Guid.CreateVersion7()))
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        
        if (string.IsNullOrWhiteSpace(animalId))
            throw new ArgumentException("Animal ID cannot be empty", nameof(animalId));
        
        if (string.IsNullOrWhiteSpace(serviceId))
            throw new ArgumentException("Service ID cannot be empty", nameof(serviceId));
        
        if (endDate.HasValue && endDate.Value.Date < startDate.Date)
            throw new ArgumentException("End date cannot be before start date");

        UserId = userId;
        AnimalId = animalId;
        AnimalName = animalName;
        ServiceId = serviceId;
        StartDate = startDate.Date;
        EndDate = endDate?.Date;
        Comments = comments;
        Status = ReservationStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public string UserId { get; private set; }
    public string AnimalId { get; private set; }
    public string AnimalName { get; private set; }
    public string ServiceId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string? Comments { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public decimal? TotalPrice { get; private set; }

    /// <summary>
    /// Gets all dates covered by this reservation
    /// </summary>
    public IEnumerable<DateTime> GetReservedDates()
    {
        var currentDate = StartDate;
        var endDate = EndDate ?? StartDate;

        while (currentDate <= endDate)
        {
            yield return currentDate;
            currentDate = currentDate.AddDays(1);
        }
    }

    /// <summary>
    /// Calculates the number of days for the reservation
    /// </summary>
    public int GetNumberOfDays()
    {
        if (!EndDate.HasValue) return 1;
        return (int)(EndDate.Value - StartDate).TotalDays + 1;
    }

    public void UpdateDates(DateTime newStartDate, DateTime? newEndDate = null)
    {
        if (Status == ReservationStatus.Completed || Status == ReservationStatus.Cancelled)
            throw new InvalidOperationException("Cannot modify dates of a completed or cancelled reservation");

        if (newEndDate.HasValue && newEndDate.Value.Date < newStartDate.Date)
            throw new ArgumentException("End date cannot be before start date");

        StartDate = newStartDate.Date;
        EndDate = newEndDate?.Date;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateComments(string? newComments)
    {
        Comments = newComments;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetTotalPrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        TotalPrice = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        if (Status != ReservationStatus.Pending)
            throw new InvalidOperationException($"Cannot confirm a reservation with status {Status}");

        Status = ReservationStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void StartService()
    {
        if (Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException($"Cannot start service for a reservation with status {Status}");

        Status = ReservationStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != ReservationStatus.InProgress)
            throw new InvalidOperationException($"Cannot complete a reservation with status {Status}");

        Status = ReservationStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == ReservationStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed reservation");

        Status = ReservationStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
