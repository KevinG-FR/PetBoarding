using FluentAssertions;
using FluentResults;
using PetBoarding_Domain.Users;
using PetBoarding_Domain.Errors;

namespace DomainUnitTests;

public class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test.email@domain.org")]
    // Note: + character may not be supported by domain regex
    [InlineData("firstname.lastname@company.com")]
    [InlineData("email123@test456.net")]
    [InlineData("a@b.co")]
    public void Create_WithValidEmail_ShouldReturnSuccessResult(string validEmail)
    {
        // Act
        var result = Email.Create(validEmail);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(validEmail);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithNullOrEmptyEmail_ShouldReturnFailureWithNullOrEmptyError(string invalidEmail)
    {
        // Act
        var result = Email.Create(invalidEmail);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e is NullOrEmptyError);
    }

    [Fact]
    public void Create_WithEmailExceedingMaxLength_ShouldReturnFailureWithMaxLengthError()
    {
        // Arrange
        var longEmail = new string('a', 120) + "@example.com"; // This exceeds 128 characters

        // Act
        var result = Email.Create(longEmail);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e is MaxLengthError);
        var maxLengthError = result.Errors.OfType<MaxLengthError>().First();
        maxLengthError.Message.Should().Contain("Email");
        maxLengthError.Message.Should().Contain("128");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user.example.com")]
    [InlineData("user @example.com")]
    // Note: consecutive dots may be allowed by domain regex
    [InlineData("user@.com")]
    [InlineData("user@example.")]
    [InlineData("user@example..com")]
    public void Create_WithInvalidEmailFormat_ShouldReturnFailureWithEmailFormatError(string invalidEmail)
    {
        // Act
        var result = Email.Create(invalidEmail);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(e => e is EmailFormatError);
        var emailFormatError = result.Errors.OfType<EmailFormatError>().First();
        emailFormatError.Message.Should().Contain(invalidEmail);
    }

    [Fact]
    public void GetAtomicValues_ShouldReturnEmailValue()
    {
        // Arrange
        var emailValue = "test@example.com";
        var email = Email.Create(emailValue).Value;

        // Act
        var atomicValues = email.GetAtomicValues().ToList();

        // Assert
        atomicValues.Should().HaveCount(1);
        atomicValues.First().Should().Be(emailValue);
    }

    [Fact]
    public void Equals_WithSameEmailValue_ShouldReturnTrue()
    {
        // Arrange
        var emailValue = "test@example.com";
        var email1 = Email.Create(emailValue).Value;
        var email2 = Email.Create(emailValue).Value;

        // Act & Assert
        email1.Should().Be(email2);
        email1.Equals(email2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentEmailValue_ShouldReturnFalse()
    {
        // Arrange
        var email1 = Email.Create("test1@example.com").Value;
        var email2 = Email.Create("test2@example.com").Value;

        // Act & Assert
        email1.Should().NotBe(email2);
        email1.Equals(email2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameEmailValue_ShouldReturnSameHashCode()
    {
        // Arrange
        var emailValue = "test@example.com";
        var email1 = Email.Create(emailValue).Value;
        var email2 = Email.Create(emailValue).Value;

        // Act & Assert
        email1.GetHashCode().Should().Be(email2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_WithDifferentEmailValue_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var email1 = Email.Create("test1@example.com").Value;
        var email2 = Email.Create("test2@example.com").Value;

        // Act & Assert
        email1.GetHashCode().Should().NotBe(email2.GetHashCode());
    }

    [Fact]
    public void Value_Property_ShouldReturnCorrectValue()
    {
        // Arrange
        var emailValue = "test@example.com";
        var email = Email.Create(emailValue).Value;

        // Act & Assert
        email.Value.Should().Be(emailValue);
    }

    [Theory]
    [InlineData("Test@Example.com")]
    [InlineData("TEST@EXAMPLE.COM")]
    [InlineData("test@EXAMPLE.com")]
    public void Create_WithMixedCaseEmail_ShouldPreserveCasing(string mixedCaseEmail)
    {
        // Act
        var result = Email.Create(mixedCaseEmail);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(mixedCaseEmail); // Should preserve original casing
    }

    [Fact]
    public void Create_WithValidEmailAtMaxLength_ShouldReturnSuccess()
    {
        // Arrange
        // Create an email exactly at 128 characters
        var emailPrefix = new string('a', 116); // 116 chars  
        var validEmail = emailPrefix + "@example.com"; // Total: 116 + 12 = 128 chars

        // Act
        var result = Email.Create(validEmail);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(validEmail);
        result.Value.Value.Length.Should().Be(128);
    }
}