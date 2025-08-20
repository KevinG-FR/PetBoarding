using PetBoarding_Domain.Abstractions;

namespace PetBoarding_Domain.Addresses
{
    public class Address : Entity<AddressId>, IAuditableEntity
    {
        public Address(StreetNumber streetNumber, StreetName streetName, City city, PostalCode postalCode, Country country, Complement? complement = null)
            : base(new AddressId(Guid.NewGuid()))
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

        // Constructeur sans paramètres pour EF Core
        private Address() : base(new AddressId(Guid.Empty)) { }

        public StreetNumber StreetNumber { get; private set; } = null!;
        public StreetName StreetName { get; private set; } = null!;
        public City City { get; private set; } = null!;
        public PostalCode PostalCode { get; private set; } = null!;
        public Country Country { get; private set; } = null!;
        public Complement? Complement { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

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
}
