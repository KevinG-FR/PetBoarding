namespace PetBoarding_Domain.Planning;

using Newtonsoft.Json;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Prestations;

/// <summary>
/// Entité représentant un planning avec des créneaux disponibles pour les prestations
/// </summary>
public sealed class Planning : Entity<PlanningId>
{
    // Constructeur privé pour création
    private Planning(PrestationId prestationId, string label, string? description = null) 
        : base(new PlanningId(Guid.CreateVersion7()))
    {
        PrestationId = prestationId;
        Label = label;
        Description = description;
        IsActive = true;
        DateCreation = DateTime.UtcNow;
        Creneaux = new List<AvailableSlot>();
    }

    // Constructeur privé pour la reconstruction depuis la cache
    [JsonConstructor]
    private Planning(PlanningId id, PrestationId prestationId, string label, List<AvailableSlot> creneaux, DateTime dateCreation, bool isActive, string? description = null)
        : base(id)
    {
        PrestationId = prestationId;
        Label = label;
        Description = description;
        IsActive = isActive;
        DateCreation = dateCreation;
        Creneaux = creneaux;
    }

    // Constructeur privé pour EF Core
    private Planning() : base()
    {
        Creneaux = [];
    }

    // Méthode factory pour créer un nouveau planning
    public static Planning Create(PrestationId prestationId, string label, string? description = null)
    {
        return new Planning(prestationId, label, description);
    }

    public PrestationId PrestationId { get; private set; }
    public string Label { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime DateCreation { get; private set; }
    public DateTime? DateModification { get; private set; }

    public List<AvailableSlot> Creneaux { get; set; }

    public void AjouterCreneau(DateTime date, int capaciteMax)
    {
        if (Creneaux.Any(c => c.Date.Date == date.Date))
        {
            throw new InvalidOperationException($"Un créneau existe déjà pour la date {date.Date:yyyy-MM-dd}");
        }

        var nouveauCreneau = AvailableSlot.Create(Id, date, capaciteMax);
        Creneaux.Add(nouveauCreneau);
        DateModification = DateTime.UtcNow;
    }

    public void AjouterCreneau(AvailableSlot creneau)
    {
        if (Creneaux.Any(c => c.Date.Date == creneau.Date.Date))
        {
            throw new InvalidOperationException($"Un créneau existe déjà pour la date {creneau.Date.Date:yyyy-MM-dd}");
        }

        creneau.AssignToPlanning(Id);
        Creneaux.Add(creneau);
        DateModification = DateTime.UtcNow;
    }

    public void DeleteSlot(DateTime date)
    {
        var creneau = Creneaux.FirstOrDefault(c => c.Date.Date == date.Date);
        if (creneau is not null)
        {
            Creneaux.Remove(creneau);
            DateModification = DateTime.UtcNow;
        }
    }

    public void UpdateSlotCapacity(DateTime date, int nouvelleCapacite)
    {
        var creneau = GetSlotForDate(date);
        if (creneau is null)
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
        return Creneaux.FirstOrDefault(c => c.Date.Date == date.Date);
    }

    public AvailableSlot? GetSlotById(AvailableSlotId slotId)
    {
        return Creneaux.FirstOrDefault(c => c.Id == slotId);
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
        if (creneau is null)
        {
            throw new InvalidOperationException($"Aucun créneau disponible pour la date {date.Date:yyyy-MM-dd}");
        }

        creneau.Reserver(quantite);
        DateModification = DateTime.UtcNow;
    }

    public void CancelReservation(DateTime date, int quantite = 1)
    {
        var creneau = GetSlotForDate(date);
        if (creneau is null)
        {
            throw new InvalidOperationException($"Aucun créneau trouvé pour la date {date.Date:yyyy-MM-dd}");
        }

        creneau.CancelReservation(quantite);
        DateModification = DateTime.UtcNow;
    }
}
