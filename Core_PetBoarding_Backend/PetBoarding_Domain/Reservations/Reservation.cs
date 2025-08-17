namespace PetBoarding_Domain.Reservations;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Planning;

/// <summary>
/// Entité représentant une réservation avec gestion du planning
/// </summary>
public sealed class Reservation : Entity<ReservationId>
{
    public Reservation(
        ReservationId id,
        string utilisateurId,
        string animalId,
        string animalNom,
        string prestationId,
        DateTime dateDebut,
        DateTime? dateFin = null,
        string? commentaires = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(utilisateurId))
            throw new ArgumentException("L'ID utilisateur ne peut pas être vide", nameof(utilisateurId));
        
        if (string.IsNullOrWhiteSpace(animalId))
            throw new ArgumentException("L'ID animal ne peut pas être vide", nameof(animalId));
        
        if (string.IsNullOrWhiteSpace(prestationId))
            throw new ArgumentException("L'ID prestation ne peut pas être vide", nameof(prestationId));
        
        if (dateFin.HasValue && dateFin.Value.Date < dateDebut.Date)
            throw new ArgumentException("La date de fin ne peut pas être antérieure à la date de début");

        UtilisateurId = utilisateurId;
        AnimalId = animalId;
        AnimalNom = animalNom;
        PrestationId = prestationId;
        DateDebut = dateDebut.Date;
        DateFin = dateFin?.Date;
        Commentaires = commentaires;
        Statut = StatutReservation.EnAttente;
        DateCreation = DateTime.UtcNow;
    }

    public string UtilisateurId { get; private set; }
    public string AnimalId { get; private set; }
    public string AnimalNom { get; private set; }
    public string PrestationId { get; private set; }
    public DateTime DateDebut { get; private set; }
    public DateTime? DateFin { get; private set; }
    public string? Commentaires { get; private set; }
    public StatutReservation Statut { get; private set; }
    public DateTime DateCreation { get; private set; }
    public DateTime? DateModification { get; private set; }
    public decimal? PrixTotal { get; private set; }

    /// <summary>
    /// Obtient toutes les dates couvertes par cette réservation
    /// </summary>
    public IEnumerable<DateTime> ObtenirDatesReservees()
    {
        var dateCourante = DateDebut;
        var dateFin = DateFin ?? DateDebut;

        while (dateCourante <= dateFin)
        {
            yield return dateCourante;
            dateCourante = dateCourante.AddDays(1);
        }
    }

    /// <summary>
    /// Calcule le nombre de jours de la réservation
    /// </summary>
    public int ObtenirNombreJours()
    {
        if (!DateFin.HasValue) return 1;
        return (int)(DateFin.Value - DateDebut).TotalDays + 1;
    }

    public void ModifierDates(DateTime nouvelleDateDebut, DateTime? nouvelleDateFin = null)
    {
        if (Statut == StatutReservation.Terminee || Statut == StatutReservation.Annulee)
            throw new InvalidOperationException("Impossible de modifier les dates d'une réservation terminée ou annulée");

        if (nouvelleDateFin.HasValue && nouvelleDateFin.Value.Date < nouvelleDateDebut.Date)
            throw new ArgumentException("La date de fin ne peut pas être antérieure à la date de début");

        DateDebut = nouvelleDateDebut.Date;
        DateFin = nouvelleDateFin?.Date;
        DateModification = DateTime.UtcNow;
    }

    public void ModifierCommentaires(string? nouveauxCommentaires)
    {
        Commentaires = nouveauxCommentaires;
        DateModification = DateTime.UtcNow;
    }

    public void DefinirPrixTotal(decimal prix)
    {
        if (prix < 0)
            throw new ArgumentException("Le prix ne peut pas être négatif", nameof(prix));

        PrixTotal = prix;
        DateModification = DateTime.UtcNow;
    }

    public void Confirmer()
    {
        if (Statut != StatutReservation.EnAttente)
            throw new InvalidOperationException($"Impossible de confirmer une réservation au statut {Statut}");

        Statut = StatutReservation.Confirmee;
        DateModification = DateTime.UtcNow;
    }

    public void DemarrerService()
    {
        if (Statut != StatutReservation.Confirmee)
            throw new InvalidOperationException($"Impossible de démarrer le service d'une réservation au statut {Statut}");

        Statut = StatutReservation.EnCours;
        DateModification = DateTime.UtcNow;
    }

    public void Terminer()
    {
        if (Statut != StatutReservation.EnCours)
            throw new InvalidOperationException($"Impossible de terminer une réservation au statut {Statut}");

        Statut = StatutReservation.Terminee;
        DateModification = DateTime.UtcNow;
    }

    public void Annuler()
    {
        if (Statut == StatutReservation.Terminee)
            throw new InvalidOperationException("Impossible d'annuler une réservation terminée");

        Statut = StatutReservation.Annulee;
        DateModification = DateTime.UtcNow;
    }
}
