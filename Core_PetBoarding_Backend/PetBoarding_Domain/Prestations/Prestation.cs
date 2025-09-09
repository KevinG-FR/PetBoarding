namespace PetBoarding_Domain.Prestations;

using Newtonsoft.Json;
using PetBoarding_Domain.Abstractions;

/// <summary>
/// Entité représentant une prestation offerte
/// </summary>
public sealed class Prestation : Entity<PrestationId>
{
    // Constructeur privé pour création
    private Prestation(
        string libelle, 
        string description, 
        TypeAnimal categorieAnimal, 
        decimal prix, 
        int dureeEnMinutes) : base(new PrestationId(Guid.CreateVersion7()))
    {
        if (string.IsNullOrWhiteSpace(libelle))
            throw new ArgumentException("Le libellé ne peut pas être vide", nameof(libelle));
        
        if (prix < 0)
            throw new ArgumentException("Le prix ne peut pas être négatif", nameof(prix));
        
        if (dureeEnMinutes <= 0)
            throw new ArgumentException("La durée doit être supérieure à 0", nameof(dureeEnMinutes));

        Libelle = libelle;
        Description = description;
        CategorieAnimal = categorieAnimal;
        Prix = prix;
        DureeEnMinutes = dureeEnMinutes;
        EstDisponible = true;
        DateCreation = DateTime.UtcNow;
    }

    // Constructeur privé pour reconstruction depuis le cache.
    [JsonConstructor]
    private Prestation(
        PrestationId id,
        string libelle,
        string description,
        TypeAnimal categorieAnimal,
        decimal prix,
        int dureeEnMinutes, bool estDisponible) : base(id)
    {
        if (string.IsNullOrWhiteSpace(libelle))
            throw new ArgumentException("Le libellé ne peut pas être vide", nameof(libelle));

        if (prix < 0)
            throw new ArgumentException("Le prix ne peut pas être négatif", nameof(prix));

        if (dureeEnMinutes <= 0)
            throw new ArgumentException("La durée doit être supérieure à 0", nameof(dureeEnMinutes));

        Libelle = libelle;
        Description = description;
        CategorieAnimal = categorieAnimal;
        Prix = prix;
        DureeEnMinutes = dureeEnMinutes;
        EstDisponible = estDisponible;
    }

    // Constructeur privé pour Entity Framework
    private Prestation() : base() { }

    // Méthode factory pour créer une nouvelle prestation
    public static Prestation Create(
        string libelle, 
        string description, 
        TypeAnimal categorieAnimal, 
        decimal prix, 
        int dureeEnMinutes)
    {
        return new Prestation(libelle, description, categorieAnimal, prix, dureeEnMinutes);
    }

    public string Libelle { get; private set; }
    public string Description { get; private set; }
    public TypeAnimal CategorieAnimal { get; private set; }
    public decimal Prix { get; private set; }
    public int DureeEnMinutes { get; private set; }
    public bool EstDisponible { get; private set; }
    public DateTime DateCreation { get; private set; }
    public DateTime? DateModification { get; private set; }

    public void ModifierLibelle(string nouveauLibelle)
    {
        if (string.IsNullOrWhiteSpace(nouveauLibelle))
            throw new ArgumentException("Le libellé ne peut pas être vide", nameof(nouveauLibelle));

        Libelle = nouveauLibelle;
        DateModification = DateTime.UtcNow;
    }

    public void ModifierDescription(string nouvelleDescription)
    {
        Description = nouvelleDescription;
        DateModification = DateTime.UtcNow;
    }

    public void ModifierPrix(decimal nouveauPrix)
    {
        if (nouveauPrix < 0)
            throw new ArgumentException("Le prix ne peut pas être négatif", nameof(nouveauPrix));

        Prix = nouveauPrix;
        DateModification = DateTime.UtcNow;
    }

    public void ModifierDuree(int nouvelleDureeEnMinutes)
    {
        if (nouvelleDureeEnMinutes <= 0)
            throw new ArgumentException("La durée doit être supérieure à 0", nameof(nouvelleDureeEnMinutes));

        DureeEnMinutes = nouvelleDureeEnMinutes;
        DateModification = DateTime.UtcNow;
    }

    public void ModifierCategorieAnimal(TypeAnimal nouvelleCategorieAnimal)
    {
        CategorieAnimal = nouvelleCategorieAnimal;
        DateModification = DateTime.UtcNow;
    }

    public void RendreDisponible()
    {
        EstDisponible = true;
        DateModification = DateTime.UtcNow;
    }

    public void RendreIndisponible()
    {
        EstDisponible = false;
        DateModification = DateTime.UtcNow;
    }

    public void Activer()
    {
        EstDisponible = true;
        DateModification = DateTime.UtcNow;
    }

    public void Desactiver()
    {
        EstDisponible = false;
        DateModification = DateTime.UtcNow;
    }
}
