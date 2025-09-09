using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Users;

namespace PetBoarding_Domain.Pets;

/// <summary>
/// Entité représentant un animal de compagnie
/// </summary>
public sealed class Pet : AuditableEntity<PetId>
{
    private Pet(
        string name,
        PetType type,
        string breed,
        int age,
        string color,
        PetGender gender,
        bool isNeutered,
        UserId ownerId,
        decimal? weight = null,
        string? microchipNumber = null,
        string? medicalNotes = null,
        string? specialNeeds = null,
        string? photoUrl = null,
        EmergencyContact? emergencyContact = null) : base(new PetId(Guid.CreateVersion7()))
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Le nom de l'animal ne peut pas être vide", nameof(name));
        
        if (string.IsNullOrWhiteSpace(breed))
            throw new ArgumentException("La race de l'animal ne peut pas être vide", nameof(breed));
        
        if (age < 0)
            throw new ArgumentException("L'âge de l'animal ne peut pas être négatif", nameof(age));
        
        if (age > 50)
            throw new ArgumentException("L'âge de l'animal ne peut pas dépasser 50 ans", nameof(age));
        
        if (string.IsNullOrWhiteSpace(color))
            throw new ArgumentException("La couleur de l'animal ne peut pas être vide", nameof(color));
        
        if (weight.HasValue && weight.Value < 0)
            throw new ArgumentException("Le poids de l'animal ne peut pas être négatif", nameof(weight));

        Name = name.Trim();
        Type = type;
        Breed = breed.Trim();
        Age = age;
        Weight = weight;
        Color = color.Trim();
        Gender = gender;
        IsNeutered = isNeutered;
        OwnerId = ownerId;
        MicrochipNumber = microchipNumber?.Trim();
        MedicalNotes = medicalNotes?.Trim();
        SpecialNeeds = specialNeeds?.Trim();
        PhotoUrl = photoUrl?.Trim();
        EmergencyContact = emergencyContact;
    }

    /// <summary>
    /// Factory method to create a new Pet
    /// </summary>
    public static Pet Create(
        string name,
        PetType type,
        string breed,
        int age,
        string color,
        PetGender gender,
        bool isNeutered,
        UserId ownerId,
        decimal? weight = null,
        string? microchipNumber = null,
        string? medicalNotes = null,
        string? specialNeeds = null,
        string? photoUrl = null,
        EmergencyContact? emergencyContact = null)
    {
        return new Pet(name, type, breed, age, color, gender, isNeutered, ownerId, weight, microchipNumber, medicalNotes, specialNeeds, photoUrl, emergencyContact);
    }

    // Constructeur pour Entity Framework
    private Pet() : base(default!)
    {
    }

    // Propriétés principales
    public string Name { get; private set; }
    public PetType Type { get; private set; }
    public string Breed { get; private set; }
    public int Age { get; private set; }
    public decimal? Weight { get; private set; }
    public string Color { get; private set; }
    public PetGender Gender { get; private set; }
    public bool IsNeutered { get; private set; }

    // Propriétés optionnelles
    public string? MicrochipNumber { get; private set; }
    public string? MedicalNotes { get; private set; }
    public string? SpecialNeeds { get; private set; }
    public string? PhotoUrl { get; private set; }

    // Relations
    public UserId OwnerId { get; private set; }
    public User? Owner { get; private set; }

    // Contact d'urgence
    public EmergencyContact? EmergencyContact { get; private set; }

    // Méthodes de modification
    public void UpdateBasicInfo(string name, string breed, int age, string color)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Le nom de l'animal ne peut pas être vide", nameof(name));
        
        if (string.IsNullOrWhiteSpace(breed))
            throw new ArgumentException("La race de l'animal ne peut pas être vide", nameof(breed));
        
        if (age < 0 || age > 50)
            throw new ArgumentException("L'âge de l'animal doit être entre 0 et 50 ans", nameof(age));
        
        if (string.IsNullOrWhiteSpace(color))
            throw new ArgumentException("La couleur de l'animal ne peut pas être vide", nameof(color));

        Name = name.Trim();
        Breed = breed.Trim();
        Age = age;
        Color = color.Trim();
    }

    public void UpdateWeight(decimal? weight)
    {
        if (weight.HasValue && weight.Value < 0)
            throw new ArgumentException("Le poids de l'animal ne peut pas être négatif", nameof(weight));

        Weight = weight;
    }

    public void UpdateType(PetType type)
    {
        Type = type;
    }

    public void UpdateGender(PetGender gender)
    {
        Gender = gender;
    }

    public void UpdateNeuteredStatus(bool isNeutered)
    {
        IsNeutered = isNeutered;
    }

    public void UpdateMicrochipNumber(string? microchipNumber)
    {
        MicrochipNumber = microchipNumber?.Trim();
    }

    public void UpdateMedicalNotes(string? medicalNotes)
    {
        MedicalNotes = medicalNotes?.Trim();
    }

    public void UpdateSpecialNeeds(string? specialNeeds)
    {
        SpecialNeeds = specialNeeds?.Trim();
    }

    public void UpdatePhotoUrl(string? photoUrl)
    {
        PhotoUrl = photoUrl?.Trim();
    }

    public void UpdateEmergencyContact(EmergencyContact? emergencyContact)
    {
        EmergencyContact = emergencyContact;
    }
}