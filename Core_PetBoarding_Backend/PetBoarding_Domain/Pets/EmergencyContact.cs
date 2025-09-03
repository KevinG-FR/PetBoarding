using PetBoarding_Domain.Abstractions;
using Newtonsoft.Json;

namespace PetBoarding_Domain.Pets;

/// <summary>
/// Value Object représentant un contact d'urgence pour un animal
/// </summary>
public sealed class EmergencyContact : ValueObject
{
    [JsonConstructor]
    public EmergencyContact(string? name, string? phone, string? relationship = null)
    {
        // Au moins le nom OU le téléphone doit être renseigné pour que le contact soit valide
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Au moins le nom ou le téléphone du contact d'urgence doit être renseigné");

        Name = name?.Trim() ?? string.Empty;
        Phone = phone?.Trim() ?? string.Empty;
        Relationship = relationship?.Trim() ?? string.Empty;
    }
    
    // Constructeur privé pour EF Core
    private EmergencyContact()
    {
        Name = string.Empty;
        Phone = string.Empty;
        Relationship = string.Empty;
    }

    public string Name { get; }
    public string Phone { get; }
    public string Relationship { get; }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Name;
        yield return Phone;
        yield return Relationship;
    }
}