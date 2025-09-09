using Newtonsoft.Json;
using PetBoarding_Domain.Abstractions;

namespace PetBoarding_Domain.Addresses;

public class Address : Entity<AddressId>, IAuditableEntity
{
    // Constructeur privé pour forcer l'utilisation de la méthode factory
    private Address(
        StreetNumber streetNumber,
        StreetName streetName,
        City city,
        PostalCode postalCode,
        Country country,
        Complement? complement = null)
            : base(new AddressId(Guid.CreateVersion7()))
    {
        StreetNumber = streetNumber;
        StreetName = streetName;
        City = city;
        PostalCode = postalCode;
        Country = country;
        Complement = complement;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    [JsonConstructor]
    private Address(
        AddressId id,
        StreetNumber streetNumber,
        StreetName streetName,
        City city,
        PostalCode postalCode,
        Country country,
        DateTime createdAt,
        DateTime updatedAt,
        Complement? complement = null)
            : base(id)
    {
        StreetNumber = streetNumber;
        StreetName = streetName;
        City = city;
        PostalCode = postalCode;
        Country = country;
        Complement = complement;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    /// <summary>
    /// Factory method to create a new Address
    /// </summary>
    public static Address Create(
        StreetNumber streetNumber,
        StreetName streetName,
        City city,
        PostalCode postalCode,
        Country country,
        Complement? complement = null)
    {
        return new Address(streetNumber, streetName, city, postalCode, country, complement);
    }

    // Constructeur sans paramètres pour EF Core
    private Address() : base(new AddressId(Guid.Empty)) { }

    public StreetNumber StreetNumber { get; private set; } = null!;
    public StreetName StreetName { get; private set; } = null!;
    public City City { get; private set; } = null!;
    public PostalCode PostalCode { get; private set; } = null!;
    public Country Country { get; private set; } = null!;
    public Complement? Complement { get; private set; }

    public readonly DateTime CreatedAt;
    public DateTime UpdatedAt { get; private set; }

    // Implémentation explicite de l'interface pour le readonly field
    DateTime IAuditableEntity.CreatedAt => CreatedAt;

    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAddress(StreetNumber streetNumber, StreetName streetName, City city, PostalCode postalCode, Country country, Complement? complement = null)
    {
        StreetNumber = streetNumber;
        StreetName = streetName;
        City = city;
        PostalCode = postalCode;
        Country = country;
        Complement = complement;
        UpdateTimestamp();
    }

    public string GetFullAddress()
    {
        var address = $"{StreetNumber.Value} {StreetName.Value}";
        if (!string.IsNullOrWhiteSpace(Complement?.Value))
            address += $", {Complement.Value}";
        address += $", {PostalCode.Value} {City.Value}, {Country.Value}";
        return address;
    }
}
