namespace PetBoarding_Domain.Planning;

using PetBoarding_Domain.Abstractions;

/// <summary>
/// Entité représentant un planning avec des créneaux disponibles pour les prestations
/// </summary>
public sealed class Planning : Entity<PlanningId>
{
    private readonly List<CreneauDisponible> _creneaux = new();

    public Planning(PlanningId id, string prestationId, string nom, string? description = null) 
        : base(id)
    {
        PrestationId = prestationId;
        Nom = nom;
        Description = description;
        EstActif = true;
        DateCreation = DateTime.UtcNow;
    }

    public string PrestationId { get; private set; }
    public string Nom { get; private set; }
    public string? Description { get; private set; }
    public bool EstActif { get; private set; }
    public DateTime DateCreation { get; private set; }
    public DateTime? DateModification { get; private set; }

    public IReadOnlyList<CreneauDisponible> Creneaux => _creneaux.AsReadOnly();

    public void AjouterCreneau(CreneauDisponible creneau)
    {
        if (_creneaux.Any(c => c.Date.Date == creneau.Date.Date))
        {
            throw new InvalidOperationException($"Un créneau existe déjà pour la date {creneau.Date.Date:yyyy-MM-dd}");
        }

        _creneaux.Add(creneau);
        DateModification = DateTime.UtcNow;
    }

    public void SupprimerCreneau(DateTime date)
    {
        var creneau = _creneaux.FirstOrDefault(c => c.Date.Date == date.Date);
        if (creneau != null)
        {
            _creneaux.Remove(creneau);
            DateModification = DateTime.UtcNow;
        }
    }

    public void ModifierNom(string nouveauNom)
    {
        Nom = nouveauNom;
        DateModification = DateTime.UtcNow;
    }

    public void ModifierDescription(string? nouvelleDescription)
    {
        Description = nouvelleDescription;
        DateModification = DateTime.UtcNow;
    }

    public void Activer()
    {
        EstActif = true;
        DateModification = DateTime.UtcNow;
    }

    public void Desactiver()
    {
        EstActif = false;
        DateModification = DateTime.UtcNow;
    }

    public CreneauDisponible? ObtenirCreneauPour(DateTime date)
    {
        return _creneaux.FirstOrDefault(c => c.Date.Date == date.Date);
    }

    public bool EstDisponiblePour(DateTime date, int quantiteDemandee = 1)
    {
        if (!EstActif) return false;

        var creneau = ObtenirCreneauPour(date);
        return creneau?.EstDisponible(quantiteDemandee) ?? false;
    }

    public void ReserverCreneau(DateTime date, int quantite = 1)
    {
        var creneau = ObtenirCreneauPour(date);
        if (creneau == null)
        {
            throw new InvalidOperationException($"Aucun créneau disponible pour la date {date.Date:yyyy-MM-dd}");
        }

        creneau.Reserver(quantite);
        DateModification = DateTime.UtcNow;
    }

    public void AnnulerReservation(DateTime date, int quantite = 1)
    {
        var creneau = ObtenirCreneauPour(date);
        if (creneau == null)
        {
            throw new InvalidOperationException($"Aucun créneau trouvé pour la date {date.Date:yyyy-MM-dd}");
        }

        creneau.AnnulerReservation(quantite);
        DateModification = DateTime.UtcNow;
    }
}
