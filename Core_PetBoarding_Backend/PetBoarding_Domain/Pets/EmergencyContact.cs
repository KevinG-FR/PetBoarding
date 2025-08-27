using PetBoarding_Domain.Abstractions;

namespace PetBoarding_Domain.Pets;

/// <summary>
/// Value Object représentant un contact d'urgence pour un animal
/// </summary>
public sealed class EmergencyContact : ValueObject
{
    public EmergencyContact(string name, string phone, string relationship)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Le nom du contact d'urgence ne peut pas être vide", nameof(name));
        
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Le téléphone du contact d'urgence ne peut pas être vide", nameof(phone));
        
        if (string.IsNullOrWhiteSpace(relationship))
            throw new ArgumentException("La relation du contact d'urgence ne peut pas être vide", nameof(relationship));

        Name = name.Trim();
        Phone = phone.Trim();
        Relationship = relationship.Trim();
    }

    public string Name { get; }
    public string Phone { get; }
    public string Relationship { get; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Phone;
        yield return Relationship;
    }
}