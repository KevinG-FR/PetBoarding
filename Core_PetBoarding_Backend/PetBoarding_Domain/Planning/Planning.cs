namespace PetBoarding_Domain.Planning;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Prestations;

/// <summary>
/// Entité représentant un planning avec des créneaux disponibles pour les prestations
/// </summary>
public sealed class Planning : Entity<PlanningId>
{
    private readonly List<AvailableSlot> _slots = new();

    public Planning(PlanningId id, PrestationId prestationId, string label, string? description = null) 
        : base(id)
    {
        PrestationId = prestationId;
        Label = label;
        Description = description;
        IsActive = true;
        DateCreation = DateTime.UtcNow;
    }

    public PrestationId PrestationId { get; private set; }
    public string Label { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime DateCreation { get; private set; }
    public DateTime? DateModification { get; private set; }

    public IReadOnlyList<AvailableSlot> Creneaux => _slots.AsReadOnly();

    public void AjouterCreneau(DateTime date, int capaciteMax)
    {
        if (_slots.Any(c => c.Date.Date == date.Date))
        {
            throw new InvalidOperationException($"Un créneau existe déjà pour la date {date.Date:yyyy-MM-dd}");
        }

        var nouveauCreneau = AvailableSlot.Create(Id, date, capaciteMax);
        _slots.Add(nouveauCreneau);
        DateModification = DateTime.UtcNow;
    }

    public void AjouterCreneau(AvailableSlot creneau)
    {
        if (_slots.Any(c => c.Date.Date == creneau.Date.Date))
        {
            throw new InvalidOperationException($"Un créneau existe déjà pour la date {creneau.Date.Date:yyyy-MM-dd}");
        }

        creneau.AssignToPlanning(Id);
        _slots.Add(creneau);
        DateModification = DateTime.UtcNow;
    }

    public void DeleteSlot(DateTime date)
    {
        var creneau = _slots.FirstOrDefault(c => c.Date.Date == date.Date);
        if (creneau != null)
        {
            _slots.Remove(creneau);
            DateModification = DateTime.UtcNow;
        }
    }

    public void UpdateSlotCapacity(DateTime date, int nouvelleCapacite)
    {
        var creneau = GetSlotForDate(date);
        if (creneau == null)
        {
            throw new InvalidOperationException($"Aucun créneau trouvé pour la date {date.Date:yyyy-MM-dd}");
        }

        creneau.UpdateCapacity(nouvelleCapacite);
        DateModification = DateTime.UtcNow;
    }

    public void ModifierNom(string nouveauNom)
    {
        Label = nouveauNom;
        DateModification = DateTime.UtcNow;
    }

    public void ModifierDescription(string? nouvelleDescription)
    {
        Description = nouvelleDescription;
        DateModification = DateTime.UtcNow;
    }

    public void Enable()
    {
        IsActive = true;
        DateModification = DateTime.UtcNow;
    }

    public void Disable()
    {
        IsActive = false;
        DateModification = DateTime.UtcNow;
    }

    public AvailableSlot? GetSlotForDate(DateTime date)
    {
        return _slots.FirstOrDefault(c => c.Date.Date == date.Date);
    }

    public bool IsAvailableForDate(DateTime date, int quantiteDemandee = 1)
    {
        if (!IsActive) return false;

        var creneau = GetSlotForDate(date);
        return creneau?.IsAvailable(quantiteDemandee) ?? false;
    }

    public void ReserveSlot(DateTime date, int quantite = 1)
    {
        var creneau = GetSlotForDate(date);
        if (creneau == null)
        {
            throw new InvalidOperationException($"Aucun créneau disponible pour la date {date.Date:yyyy-MM-dd}");
        }

        creneau.Reserver(quantite);
        DateModification = DateTime.UtcNow;
    }

    public void CancelReservation(DateTime date, int quantite = 1)
    {
        var creneau = GetSlotForDate(date);
        if (creneau == null)
        {
            throw new InvalidOperationException($"Aucun créneau trouvé pour la date {date.Date:yyyy-MM-dd}");
        }

        creneau.CancelReservation(quantite);
        DateModification = DateTime.UtcNow;
    }
}
