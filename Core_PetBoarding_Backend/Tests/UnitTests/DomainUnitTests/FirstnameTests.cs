using FluentAssertions;
using FluentResults;
using PetBoarding_Domain.Users;
using PetBoarding_Domain.Errors;

namespace DomainUnitTests;

public class FirstnameTests
{
    [Theory]
    [InlineData("Jean")]
    [InlineData("Marie")]
    [InlineData("Pierre-Alexandre")]
    [InlineData("Anne-Marie")]
    [InlineData("Jean Claude")]
    [InlineData("A")]
    [InlineData("Jean-Baptiste-Emmanuel")]
    public void Create_WithValidFirstname_ShouldReturnSuccessResult(string validFirstname)
    {
        // Act
        var result = Firstname.Create(validFirstname);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(validFirstname);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData(null)]
    public void Create_WithNullOrWhitespaceFirstname_ShouldReturnFailureWithNullOrEmptyError(string invalidFirstname)
    {
        // Act
        var result = Firstname.Create(invalidFirstname);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e is NullOrEmptyError);
        var error = result.Errors.OfType<NullOrEmptyError>().First();
        error.Message.Should().Contain("Firstname");
    }

    [Fact]
    public void Create_WithFirstnameExceedingMaxLength_ShouldReturnFailureWithMaxLengthError()
    {
        // Arrange
        var longFirstname = new string('A', 51); // Exceeds 50 characters

        // Act
        var result = Firstname.Create(longFirstname);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e is MaxLengthError);
        var maxLengthError = result.Errors.OfType<MaxLengthError>().First();
        maxLengthError.Message.Should().Contain("Firstname");
        maxLengthError.Message.Should().Contain("50");
    }

    [Fact]
    public void Create_WithFirstnameAtMaxLength_ShouldReturnSuccess()
    {
        // Arrange
        var firstnameAtMaxLength = new string('A', 50); // Exactly 50 characters

        // Act
        var result = Firstname.Create(firstnameAtMaxLength);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(firstnameAtMaxLength);
        result.Value.Value.Length.Should().Be(50);
    }

    [Fact]
    public void GetAtomicValues_ShouldReturnFirstnameValue()
    {
        // Arrange
        var firstnameValue = "Jean";
        var firstname = Firstname.Create(firstnameValue).Value;

        // Act
        var atomicValues = firstname.GetAtomicValues().ToList();

        // Assert
        atomicValues.Should().HaveCount(1);
        atomicValues.First().Should().Be(firstnameValue);
    }

    [Fact]
    public void Equals_WithSameFirstnameValue_ShouldReturnTrue()
    {
        // Arrange
        var firstnameValue = "Jean";
        var firstname1 = Firstname.Create(firstnameValue).Value;
        var firstname2 = Firstname.Create(firstnameValue).Value;

        // Act & Assert
        firstname1.Should().Be(firstname2);
        firstname1.Equals(firstname2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentFirstnameValue_ShouldReturnFalse()
    {
        // Arrange
        var firstname1 = Firstname.Create("Jean").Value;
        var firstname2 = Firstname.Create("Pierre").Value;

        // Act & Assert
        firstname1.Should().NotBe(firstname2);
        firstname1.Equals(firstname2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameFirstnameValue_ShouldReturnSameHashCode()
    {
        // Arrange
        var firstnameValue = "Jean";
        var firstname1 = Firstname.Create(firstnameValue).Value;
        var firstname2 = Firstname.Create(firstnameValue).Value;

        // Act & Assert
        firstname1.GetHashCode().Should().Be(firstname2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentFirstnameValue_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var firstname1 = Firstname.Create("Jean").Value;
        var firstname2 = Firstname.Create("Pierre").Value;

        // Act & Assert
        firstname1.GetHashCode().Should().NotBe(firstname2.GetHashCode());
    }

    [Fact]
    public void Value_Property_ShouldReturnCorrectValue()
    {
        // Arrange
        var firstnameValue = "Jean";
        var firstname = Firstname.Create(firstnameValue).Value;

        // Act & Assert
        firstname.Value.Should().Be(firstnameValue);
    }

    [Theory]
    [InlineData("jean", "jean")]
    [InlineData("JEAN", "JEAN")]
    [InlineData("Jean", "Jean")]
    [InlineData("jEaN", "jEaN")]
    public void Create_ShouldPreserveCasing(string input, string expected)
    {
        // Act
        var result = Firstname.Create(input);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData("Jean-Baptiste")]
    [InlineData("Anne-Marie")]
    [InlineData("Pierre-Alexandre")]
    [InlineData("Marie-Claire")]
    public void Create_WithHyphenatedNames_ShouldReturnSuccess(string hyphenatedName)
    {
        // Act
        var result = Firstname.Create(hyphenatedName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(hyphenatedName);
    }

    [Theory]
    [InlineData("Jean Claude")]
    [InlineData("Marie Antoinette")]
    [InlineData("Anna Maria")]
    public void Create_WithSpacedNames_ShouldReturnSuccess(string spacedName)
    {
        // Act
        var result = Firstname.Create(spacedName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(spacedName);
    }
}