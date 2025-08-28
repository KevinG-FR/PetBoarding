namespace PetBoarding_Domain.Reservations;

using PetBoarding_Domain.Abstractions;

/// <summary>
/// Entité de liaison entre une réservation et un créneau disponible spécifique
/// Permet une traçabilité exacte de quels créneaux ont été réservés
/// </summary>
public sealed class ReservationSlot : Entity<ReservationSlotId>
{
    public ReservationSlot(
        ReservationId reservationId,
        Guid availableSlotId) : base(new ReservationSlotId(Guid.CreateVersion7()))
    {
        if (reservationId == null)
            throw new ArgumentNullException(nameof(reservationId));

        if (availableSlotId == Guid.Empty)
            throw new ArgumentException("AvailableSlotId cannot be empty", nameof(availableSlotId));

        ReservationId = reservationId;
        AvailableSlotId = availableSlotId;
        ReservedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// ID de la réservation qui possède ce créneau
    /// </summary>
    public ReservationId ReservationId { get; private set; }

    /// <summary>
    /// ID du créneau disponible qui a été réservé
    /// </summary>
    public Guid AvailableSlotId { get; private set; }

    /// <summary>
    /// Date et heure à laquelle ce créneau a été réservé (audit)
    /// </summary>
    public DateTime ReservedAt { get; private set; }

    /// <summary>
    /// Date optionnelle de libération du créneau (pour audit des annulations)
    /// </summary>
    public DateTime? ReleasedAt { get; private set; }

    /// <summary>
    /// Marque ce créneau comme libéré (pour audit)
    /// </summary>
    public void MarkAsReleased()
    {
        ReleasedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Vérifie si ce créneau est encore actif (non libéré)
    /// </summary>
    public bool IsActive => ReleasedAt == null;
}