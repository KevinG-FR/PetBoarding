using FluentAssertions;
using PetBoarding_Domain.Addresses;
using PetBoarding_Domain.Abstractions;

namespace DomainUnitTests;

public class AddressTests
{
    private readonly StreetNumber _validStreetNumber;
    private readonly StreetName _validStreetName;
    private readonly City _validCity;
    private readonly PostalCode _validPostalCode;
    private readonly Country _validCountry;
    private readonly Complement _validComplement;

    public AddressTests()
    {
        _validStreetNumber = StreetNumber.Create("123").Value;
        _validStreetName = StreetName.Create("Rue de la Paix").Value;
        _validCity = City.Create("Paris").Value;
        _validPostalCode = PostalCode.Create("75001").Value;
        _validCountry = Country.Create("France").Value;
        _validComplement = Complement.Create("Appartement 4B").Value;
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateAddress()
    {
        // Act
        var address = new Address(_validStreetNumber, _validStreetName, _validCity, _validPostalCode, _validCountry);

        // Assert
        address.Should().NotBeNull();
        address.Id.Should().NotBeNull();
        address.StreetNumber.Should().Be(_validStreetNumber);
        address.StreetName.Should().Be(_validStreetName);
        address.City.Should().Be(_validCity);
        address.PostalCode.Should().Be(_validPostalCode);
        address.Country.Should().Be(_validCountry);
        address.Complement.Should().BeNull();
        address.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        address.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithComplement_ShouldCreateAddressWithComplement()
    {
        // Act
        var address = new Address(_validStreetNumber, _validStreetName, _validCity, _validPostalCode, _validCountry, _validComplement);

        // Assert
        address.Should().NotBeNull();
        address.Id.Should().NotBeNull();
        address.StreetNumber.Should().Be(_validStreetNumber);
        address.StreetName.Should().Be(_validStreetName);
        address.City.Should().Be(_validCity);
        address.PostalCode.Should().Be(_validPostalCode);
        address.Country.Should().Be(_validCountry);
        address.Complement.Should().Be(_validComplement);
        address.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        address.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateTimestamp_ShouldUpdateUpdatedAtProperty()
    {
        // Arrange
        var address = new Address(_validStreetNumber, _validStreetName, _validCity, _validPostalCode, _validCountry);
        var initialUpdatedAt = address.UpdatedAt;
        
        // Wait a small amount to ensure time difference
        Thread.Sleep(1);

        // Act
        address.UpdateTimestamp();

        // Assert
        address.UpdatedAt.Should().BeAfter(initialUpdatedAt);
        address.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateAddress_WithValidParameters_ShouldUpdateAllProperties()
    {
        // Arrange
        var address = new Address(_validStreetNumber, _validStreetName, _validCity, _validPostalCode, _validCountry);
        var initialUpdatedAt = address.UpdatedAt;
        
        var newStreetNumber = StreetNumber.Create("456").Value;
        var newStreetName = StreetName.Create("Avenue des Champs").Value;
        var newCity = City.Create("Lyon").Value;
        var newPostalCode = PostalCode.Create("69000").Value;
        var newCountry = Country.Create("France").Value;
        var newComplement = Complement.Create("Bâtiment A").Value;

        Thread.Sleep(1); // Ensure time difference

        // Act
        address.UpdateAddress(newStreetNumber, newStreetName, newCity, newPostalCode, newCountry, newComplement);

        // Assert
        address.StreetNumber.Should().Be(newStreetNumber);
        address.StreetName.Should().Be(newStreetName);
        address.City.Should().Be(newCity);
        address.PostalCode.Should().Be(newPostalCode);
        address.Country.Should().Be(newCountry);
        address.Complement.Should().Be(newComplement);
        address.UpdatedAt.Should().BeAfter(initialUpdatedAt);
    }

    [Fact]
    public void UpdateAddress_WithoutComplement_ShouldUpdatePropertiesAndSetComplementToNull()
    {
        // Arrange
        var address = new Address(_validStreetNumber, _validStreetName, _validCity, _validPostalCode, _validCountry, _validComplement);
        var initialUpdatedAt = address.UpdatedAt;
        
        var newStreetNumber = StreetNumber.Create("456").Value;
        var newStreetName = StreetName.Create("Avenue des Champs").Value;
        var newCity = City.Create("Lyon").Value;
        var newPostalCode = PostalCode.Create("69000").Value;
        var newCountry = Country.Create("France").Value;

        Thread.Sleep(1); // Ensure time difference

        // Act
        address.UpdateAddress(newStreetNumber, newStreetName, newCity, newPostalCode, newCountry);

        // Assert
        address.StreetNumber.Should().Be(newStreetNumber);
        address.StreetName.Should().Be(newStreetName);
        address.City.Should().Be(newCity);
        address.PostalCode.Should().Be(newPostalCode);
        address.Country.Should().Be(newCountry);
        address.Complement.Should().BeNull();
        address.UpdatedAt.Should().BeAfter(initialUpdatedAt);
    }

    [Fact]
    public void GetFullAddress_WithoutComplement_ShouldReturnCorrectFormat()
    {
        // Arrange
        var address = new Address(_validStreetNumber, _validStreetName, _validCity, _validPostalCode, _validCountry);

        // Act
        var fullAddress = address.GetFullAddress();

        // Assert
        fullAddress.Should().Be("123 Rue de la Paix, 75001 Paris, France");
    }

    [Fact]
    public void GetFullAddress_WithComplement_ShouldReturnCorrectFormat()
    {
        // Arrange
        var address = new Address(_validStreetNumber, _validStreetName, _validCity, _validPostalCode, _validCountry, _validComplement);

        // Act
        var fullAddress = address.GetFullAddress();

        // Assert
        fullAddress.Should().Be("123 Rue de la Paix, Appartement 4B, 75001 Paris, France");
    }

    [Fact]
    public void GetFullAddress_WithEmptyComplement_ShouldReturnCorrectFormat()
    {
        // Arrange
        var emptyComplement = Complement.Create("").Value;
        var address = new Address(_validStreetNumber, _validStreetName, _validCity, _validPostalCode, _validCountry, emptyComplement);

        // Act
        var fullAddress = address.GetFullAddress();

        // Assert
        fullAddress.Should().Be("123 Rue de la Paix, 75001 Paris, France");
    }

    [Fact]
    public void GetFullAddress_WithWhitespaceComplement_ShouldReturnCorrectFormat()
    {
        // Arrange
        var whitespaceComplement = Complement.Create("   ").Value;
        var address = new Address(_validStreetNumber, _validStreetName, _validCity, _validPostalCode, _validCountry, whitespaceComplement);

        // Act
        var fullAddress = address.GetFullAddress();

        // Assert
        fullAddress.Should().Be("123 Rue de la Paix, 75001 Paris, France");
    }

    [Fact]
    public void CreatedAt_ShouldBeReadonly()
    {
        // Arrange
        var address = new Address(_validStreetNumber, _validStreetName, _validCity, _validPostalCode, _validCountry);
        var initialCreatedAt = address.CreatedAt;
        
        // Wait to ensure time difference
        Thread.Sleep(10);

        // Act
        address.UpdateTimestamp();

        // Assert
        address.CreatedAt.Should().Be(initialCreatedAt); // Should not change
        address.UpdatedAt.Should().BeAfter(initialCreatedAt); // Should change
    }

    [Fact]
    public void IAuditableEntity_CreatedAt_ShouldReturnSameValueAsReadonlyField()
    {
        // Arrange
        var address = new Address(_validStreetNumber, _validStreetName, _validCity, _validPostalCode, _validCountry);

        // Act
        var auditableEntity = (IAuditableEntity)address;

        // Assert
        auditableEntity.CreatedAt.Should().Be(address.CreatedAt);
    }

    [Fact]
    public void Constructor_ShouldSetCreatedAtAndUpdatedAtToSameTime()
    {
        // Act
        var address = new Address(_validStreetNumber, _validStreetName, _validCity, _validPostalCode, _validCountry);

        // Assert
        address.CreatedAt.Should().BeCloseTo(address.UpdatedAt, TimeSpan.FromMilliseconds(1));
    }
}