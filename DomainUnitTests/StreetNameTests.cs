using FluentAssertions;
using FluentResults;
using PetBoarding_Domain.Addresses;
using PetBoarding_Domain.Errors;

namespace DomainUnitTests;

public class StreetNameTests
{
    [Theory]
    [InlineData("Rue de la Paix")]
    [InlineData("Avenue des Champs-Élysées")]
    [InlineData("Boulevard Saint-Germain")]
    [InlineData("Place de la République")]
    [InlineData("Impasse du Soleil")]
    [InlineData("Allée des Roses")]
    [InlineData("Chemin des Vignes")]
    [InlineData("Main Street")]
    [InlineData("Oak Avenue")]
    [InlineData("A")]
    public void Create_WithValidStreetName_ShouldReturnSuccessResult(string validStreetName)
    {
        // Act
        var result = StreetName.Create(validStreetName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(validStreetName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData(null)]
    public void Create_WithNullOrWhitespaceStreetName_ShouldReturnFailureWithNullOrEmptyError(string invalidStreetName)
    {
        // Act
        var result = StreetName.Create(invalidStreetName);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e is NullOrEmptyError);
        var error = result.Errors.OfType<NullOrEmptyError>().First();
        error.Message.Should().Contain("StreetName");
    }

    [Fact]
    public void Create_WithStreetNameExceedingMaxLength_ShouldReturnFailureWithMaxLengthError()
    {
        // Arrange
        var longStreetName = new string('A', 101); // Exceeds 100 characters

        // Act
        var result = StreetName.Create(longStreetName);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e is MaxLengthError);
        var maxLengthError = result.Errors.OfType<MaxLengthError>().First();
        maxLengthError.Message.Should().Contain("StreetName");
        maxLengthError.Message.Should().Contain("100");
    }

    [Fact]
    public void Create_WithStreetNameAtMaxLength_ShouldReturnSuccess()
    {
        // Arrange
        var streetNameAtMaxLength = new string('A', 100); // Exactly 100 characters

        // Act
        var result = StreetName.Create(streetNameAtMaxLength);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(streetNameAtMaxLength);
        result.Value.Value.Length.Should().Be(100);
    }

    [Fact]
    public void GetAtomicValues_ShouldReturnStreetNameValue()
    {
        // Arrange
        var streetNameValue = "Rue de la Paix";
        var streetName = StreetName.Create(streetNameValue).Value;

        // Act
        var atomicValues = streetName.GetAtomicValues().ToList();

        // Assert
        atomicValues.Should().HaveCount(1);
        atomicValues.First().Should().Be(streetNameValue);
    }

    [Fact]
    public void Equals_WithSameStreetNameValue_ShouldReturnTrue()
    {
        // Arrange
        var streetNameValue = "Rue de la Paix";
        var streetName1 = StreetName.Create(streetNameValue).Value;
        var streetName2 = StreetName.Create(streetNameValue).Value;

        // Act & Assert
        streetName1.Should().Be(streetName2);
        streetName1.Equals(streetName2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentStreetNameValue_ShouldReturnFalse()
    {
        // Arrange
        var streetName1 = StreetName.Create("Rue de la Paix").Value;
        var streetName2 = StreetName.Create("Avenue des Champs").Value;

        // Act & Assert
        streetName1.Should().NotBe(streetName2);
        streetName1.Equals(streetName2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameStreetNameValue_ShouldReturnSameHashCode()
    {
        // Arrange
        var streetNameValue = "Rue de la Paix";
        var streetName1 = StreetName.Create(streetNameValue).Value;
        var streetName2 = StreetName.Create(streetNameValue).Value;

        // Act & Assert
        streetName1.GetHashCode().Should().Be(streetName2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentStreetNameValue_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var streetName1 = StreetName.Create("Rue de la Paix").Value;
        var streetName2 = StreetName.Create("Avenue des Champs").Value;

        // Act & Assert
        streetName1.GetHashCode().Should().NotBe(streetName2.GetHashCode());
    }

    [Fact]
    public void Value_Property_ShouldReturnCorrectValue()
    {
        // Arrange
        var streetNameValue = "Rue de la Paix";
        var streetName = StreetName.Create(streetNameValue).Value;

        // Act & Assert
        streetName.Value.Should().Be(streetNameValue);
    }

    [Theory]
    [InlineData("rue de la paix", "rue de la paix")]
    [InlineData("RUE DE LA PAIX", "RUE DE LA PAIX")]
    [InlineData("Rue de la Paix", "Rue de la Paix")]
    [InlineData("RuE dE lA PaIx", "RuE dE lA PaIx")]
    public void Create_ShouldPreserveCasing(string input, string expected)
    {
        // Act
        var result = StreetName.Create(input);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData("Rue Saint-Germain")]
    [InlineData("Avenue Jean-Jaurès")]
    [InlineData("Boulevard Victor-Hugo")]
    [InlineData("Place Charles-de-Gaulle")]
    public void Create_WithHyphenatedStreetNames_ShouldReturnSuccess(string hyphenatedStreetName)
    {
        // Act
        var result = StreetName.Create(hyphenatedStreetName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(hyphenatedStreetName);
    }

    [Theory]
    [InlineData("Rue de la République")]
    [InlineData("Avenue du Général Leclerc")]
    [InlineData("Boulevard de la Liberté")]
    [InlineData("Place du Marché aux Fleurs")]
    public void Create_WithMultiWordStreetNames_ShouldReturnSuccess(string multiWordStreetName)
    {
        // Act
        var result = StreetName.Create(multiWordStreetName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(multiWordStreetName);
    }

    [Theory]
    [InlineData("Rue des Champs-Élysées")]
    [InlineData("Avenue François 1er")]
    [InlineData("Place Sainte-Geneviève")]
    [InlineData("Boulevard du 14 Juillet")]
    public void Create_WithSpecialCharacters_ShouldReturnSuccess(string streetNameWithSpecialChars)
    {
        // Act
        var result = StreetName.Create(streetNameWithSpecialChars);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(streetNameWithSpecialChars);
    }

    [Theory]
    [InlineData("123 Main Street")] // Street number included
    [InlineData("Apt 4B Oak Avenue")] // Apartment info included  
    [InlineData("Suite 200 Business Blvd")] // Suite info included
    public void Create_WithAdditionalAddressInfo_ShouldStillReturnSuccess(string streetNameWithExtra)
    {
        // Act
        var result = StreetName.Create(streetNameWithExtra);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(streetNameWithExtra);
    }
}