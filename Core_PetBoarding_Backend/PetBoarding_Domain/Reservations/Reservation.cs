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
        Status = ReservationStatus.Created;
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
    public DateTime? PaidAt { get; private set; }
    
    /// <summary>
    /// Collection des créneaux spécifiques réservés pour cette réservation
    /// Remplace progressivement la logique StartDate/EndDate
    /// </summary>
    private readonly List<ReservationSlot> _reservedSlots = new();
    public IReadOnlyCollection<ReservationSlot> ReservedSlots => _reservedSlots.AsReadOnly();

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

    /// <summary>
    /// Marks the reservation as paid when the basket payment is successful
    /// </summary>
    public void MarkAsPaid()
    {
        if (Status != ReservationStatus.Created)
            throw new InvalidOperationException($"Cannot mark as paid a reservation with status {Status}");

        Status = ReservationStatus.Validated;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }



    public void StartService()
    {
        if (Status != ReservationStatus.Validated)
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

    /// <summary>
    /// Annule manuellement la réservation (par le client)
    /// </summary>
    public void Cancel()
    {
        if (Status == ReservationStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed reservation");
        
        if (Status == ReservationStatus.CancelAuto)
            throw new InvalidOperationException("Reservation is already auto-cancelled");

        Status = ReservationStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Vérifie si la réservation nécessite une libération de créneaux
    /// </summary>
    public bool RequiresSlotRelease()
    {
        return Status is ReservationStatus.Cancelled or ReservationStatus.CancelAuto;
    }

    /// <summary>
    /// Vérifie si la réservation occupe activement des créneaux
    /// </summary>
    public bool IsActivelyReserved()
    {
        return Status is ReservationStatus.Created or ReservationStatus.Validated or ReservationStatus.InProgress;
    }

    #region Nouvelle logique de gestion des créneaux

    /// <summary>
    /// Ajoute un créneau spécifique à cette réservation
    /// </summary>
    public void AddReservedSlot(Guid availableSlotId)
    {
        if (availableSlotId == Guid.Empty)
            throw new ArgumentException("AvailableSlotId cannot be empty", nameof(availableSlotId));

        // Vérifier qu'on ne réserve pas le même créneau deux fois
        if (_reservedSlots.Any(rs => rs.AvailableSlotId == availableSlotId && rs.IsActive))
            throw new InvalidOperationException($"Slot {availableSlotId} is already reserved for this reservation");

        var reservationSlot = new ReservationSlot(Id, availableSlotId);
        _reservedSlots.Add(reservationSlot);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Libère un créneau spécifique de cette réservation
    /// </summary>
    public void ReleaseReservedSlot(Guid availableSlotId)
    {
        var reservationSlot = _reservedSlots
            .FirstOrDefault(rs => rs.AvailableSlotId == availableSlotId && rs.IsActive);

        if (reservationSlot == null)
            throw new InvalidOperationException($"Slot {availableSlotId} is not actively reserved for this reservation");

        reservationSlot.MarkAsReleased();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Libère tous les créneaux actifs de cette réservation
    /// </summary>
    public void ReleaseAllReservedSlots()
    {
        var activeSlots = _reservedSlots.Where(rs => rs.IsActive).ToList();
        
        foreach (var slot in activeSlots)
        {
            slot.MarkAsReleased();
        }

        if (activeSlots.Any())
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Obtient tous les IDs des créneaux actuellement réservés (actifs)
    /// </summary>
    public IEnumerable<Guid> GetActiveReservedSlotIds()
    {
        return _reservedSlots
            .Where(rs => rs.IsActive)
            .Select(rs => rs.AvailableSlotId);
    }

    /// <summary>
    /// Vérifie si un créneau spécifique est réservé par cette réservation
    /// </summary>
    public bool IsSlotReserved(Guid availableSlotId)
    {
        return _reservedSlots.Any(rs => rs.AvailableSlotId == availableSlotId && rs.IsActive);
    }

    /// <summary>
    /// Obtient le nombre de créneaux actuellement réservés
    /// </summary>
    public int GetActiveReservedSlotsCount()
    {
        return _reservedSlots.Count(rs => rs.IsActive);
    }

    #endregion
}
