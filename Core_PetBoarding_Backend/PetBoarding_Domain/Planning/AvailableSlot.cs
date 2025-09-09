namespace PetBoarding_Domain.Planning;

using PetBoarding_Domain.Abstractions;

/// <summary>
/// Entité représentant un créneau disponible pour une date donnée
/// </summary>
public sealed class AvailableSlot : Entity<AvailableSlotId>
{
    // Constructeur privé pour EF Core
    private AvailableSlot() : base(default!) { }

    private AvailableSlot(AvailableSlotId id, PlanningId planningId, DateTime date, int capaciteMax, int capaciteReservee = 0)
        : base(id)
    {
        if (capaciteMax <= 0)
        {
            throw new ArgumentException("La capacité maximale doit être supérieure à 0", nameof(capaciteMax));
        }

        if (capaciteReservee < 0)
        {
            throw new ArgumentException("La capacité réservée ne peut pas être négative", nameof(capaciteReservee));
        }

        if (capaciteReservee > capaciteMax)
        {
            throw new ArgumentException("La capacité réservée ne peut pas dépasser la capacité maximale", nameof(capaciteReservee));
        }

        PlanningId = planningId;
        Date = date.Date; // On ne garde que la partie date
        MaxCapacity = capaciteMax;
        CapaciteReservee = capaciteReservee;
        CreatedAt = DateTime.UtcNow;
    }

    public PlanningId PlanningId { get; private set; }
    public DateTime Date { get; private set; }
    public int MaxCapacity { get; private set; }
    public int CapaciteReservee { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ModifiedAt { get; private set; }

    public int AvailableCapacity => MaxCapacity - CapaciteReservee;

    public bool IsAvailable(int quantiteDemandee = 1)
    {
        return AvailableCapacity >= quantiteDemandee && Date.Date >= DateTime.Today;
    }

    public void Reserver(int quantite)
    {
        if (quantite <= 0)
        {
            throw new ArgumentException("La quantité à réserver doit être supérieure à 0", nameof(quantite));
        }

        if (!IsAvailable(quantite))
        {
            throw new InvalidOperationException($"Capacité insuffisante. Disponible: {AvailableCapacity}, Demandée: {quantite}");
        }

        CapaciteReservee += quantite;
        ModifiedAt = DateTime.UtcNow;
    }

    public void CancelReservation(int quantite)
    {
        if (quantite <= 0)
        {
            throw new ArgumentException("La quantité à annuler doit être supérieure à 0", nameof(quantite));
        }

        if (quantite > CapaciteReservee)
        {
            throw new InvalidOperationException($"Impossible d'annuler {quantite} réservations. Seulement {CapaciteReservee} réservations actives.");
        }

        CapaciteReservee -= quantite;
        ModifiedAt = DateTime.UtcNow;
    }

    public void UpdateCapacity(int nouvelleCapaciteMax)
    {
        if (nouvelleCapaciteMax <= 0)
        {
            throw new ArgumentException("La capacité maximale doit être supérieure à 0", nameof(nouvelleCapaciteMax));
        }

        MaxCapacity = nouvelleCapaciteMax;
        // Ajuster la capacité réservée si elle dépasse la nouvelle capacité max
        if (CapaciteReservee > nouvelleCapaciteMax)
        {
            CapaciteReservee = nouvelleCapaciteMax;
        }
        ModifiedAt = DateTime.UtcNow;
    }

    internal void AssignToPlanning(PlanningId planningId)
    {
        PlanningId = planningId;
        ModifiedAt = DateTime.UtcNow;
    }

    // Factory method pour créer un nouveau slot
    public static AvailableSlot Create(PlanningId planningId, DateTime date, int capaciteMax, int capaciteReservee = 0)
    {
        return new AvailableSlot(AvailableSlotId.New(), planningId, date, capaciteMax, capaciteReservee);
    }

    public override string ToString()
    {
        return $"Créneau {Date:yyyy-MM-dd} - {AvailableCapacity}/{MaxCapacity} disponible (Planning: {PlanningId})";
    }
}
