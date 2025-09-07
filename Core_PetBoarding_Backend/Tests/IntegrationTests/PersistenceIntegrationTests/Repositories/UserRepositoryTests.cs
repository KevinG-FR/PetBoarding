using FluentAssertions;
using PetBoarding_Domain.Addresses;
using PetBoarding_Domain.Users;
using PetBoarding_Persistence.Repositories;

namespace PersistenceIntegrationTests.Repositories;

public class UserRepositoryTests : PostgreSqlTestBase
{
    private UserRepository _repository => new UserRepository(Context);

    [Fact]
    public async Task GetByIdAsync_Should_IncludeAddress_When_UserHasAddress()
    {
        // Arrange        
        var address = new Address(
            StreetNumber.Create("123").Value,
            StreetName.Create("Test Street").Value,
            City.Create("Test City").Value,
            PostalCode.Create("12345").Value,
            Country.Create("Test Country").Value
        );
        
        var user = new User(
            Firstname.Create("John").Value,
            Lastname.Create("Doe").Value,
            Email.Create("john.doe@test.com").Value,
            PhoneNumber.Create("+33123456789").Value,
            "hashedpassword",
            UserProfileType.Customer
        );

        user.AddressId = address.Id;

        Context.Addresses.Add(address);
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Address.Should().NotBeNull();
        result.Address!.StreetNumber.Value.Should().Be("123");
        result.Address!.StreetName.Value.Should().Be("Test Street");
    }

    [Fact]
    public async Task UserEmailAlreadyUsed_Should_ReturnTrue_When_EmailExists()
    {
        // Arrange
        const string email = "john.doe@test.com";
        
        var user = new User(
            Firstname.Create("John").Value,
            Lastname.Create("Doe").Value,
            Email.Create(email).Value,
            PhoneNumber.Create("+33123456789").Value,
            "hashedpassword",
            UserProfileType.Customer
        );

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.UserEmailAlreadyUsed(email, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UserEmailAlreadyUsed_Should_ReturnFalse_When_EmailDoesNotExist()
    {
        // Arrange
        const string email = "nonexistent@test.com";

        // Act
        var result = await _repository.UserEmailAlreadyUsed(email, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserForAuthentification_Should_ReturnUser_When_CredentialsAreCorrect()
    {
        // Arrange
        const string email = "john.doe@test.com";
        const string passwordHash = "hashedpassword";
        
        var user = new User(
            Firstname.Create("John").Value,
            Lastname.Create("Doe").Value,
            Email.Create(email).Value,
            PhoneNumber.Create("+33123456789").Value,
            passwordHash,
            UserProfileType.Customer
        );

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserForAuthentification(email, passwordHash, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Value.Should().Be(email);
        result.PasswordHash.Should().Be(passwordHash);
    }

    [Fact]
    public async Task GetUserForAuthentification_Should_ReturnNull_When_EmailIsWrong()
    {
        // Arrange
        const string email = "john.doe@test.com";
        const string wrongEmail = "wrong@test.com";
        const string passwordHash = "hashedpassword";
        
        var user = new User(
            Firstname.Create("John").Value,
            Lastname.Create("Doe").Value,
            Email.Create(email).Value,
            PhoneNumber.Create("+33123456789").Value,
            passwordHash,
            UserProfileType.Customer
        );

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserForAuthentification(wrongEmail, passwordHash, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserForAuthentification_Should_ReturnNull_When_PasswordIsWrong()
    {
        // Arrange
        const string email = "john.doe@test.com";
        const string passwordHash = "hashedpassword";
        const string wrongPasswordHash = "wrongpassword";
        
        var user = new User(
            Firstname.Create("John").Value,
            Lastname.Create("Doe").Value,
            Email.Create(email).Value,
            PhoneNumber.Create("+33123456789").Value,
            passwordHash,
            UserProfileType.Customer
        );

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserForAuthentification(email, wrongPasswordHash, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserForAuthentification_Should_IncludeAddress()
    {
        // Arrange
        const string email = "john.doe@test.com";
        const string passwordHash = "hashedpassword";
        
        var address = new Address(
            StreetNumber.Create("123").Value,
            StreetName.Create("Test Street").Value,
            City.Create("Test City").Value,
            PostalCode.Create("12345").Value,
            Country.Create("Test Country").Value
        );
        
        var user = new User(
            Firstname.Create("John").Value,
            Lastname.Create("Doe").Value,
            Email.Create(email).Value,
            PhoneNumber.Create("+33123456789").Value,
            passwordHash,
            UserProfileType.Customer
        );

        user.AddressId = address.Id;

        Context.Addresses.Add(address);
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserForAuthentification(email, passwordHash, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Address.Should().NotBeNull();
        result.Address!.StreetNumber.Value.Should().Be("123");
        result.Address!.StreetName.Value.Should().Be("Test Street");
    }

    [Fact]
    public async Task GetByEmailAsync_Should_ReturnUser_When_EmailExists()
    {
        // Arrange
        var email = Email.Create("john.doe@test.com").Value;
        
        var user = new User(
            Firstname.Create("John").Value,
            Lastname.Create("Doe").Value,
            email,
            PhoneNumber.Create("+33123456789").Value,
            "hashedpassword",
            UserProfileType.Customer
        );

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(email, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
    }

    [Fact]
    public async Task GetByEmailAsync_Should_ReturnNull_When_EmailDoesNotExist()
    {
        // Arrange
        var email = Email.Create("nonexistent@test.com").Value;

        // Act
        var result = await _repository.GetByEmailAsync(email, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_Should_IncludeAddress()
    {
        // Arrange
        var email = Email.Create("john.doe@test.com").Value;
        
        var address = new Address(
            StreetNumber.Create("123").Value,
            StreetName.Create("Test Street").Value,
            City.Create("Test City").Value,
            PostalCode.Create("12345").Value,
            Country.Create("Test Country").Value
        );
        
        var user = new User(
            Firstname.Create("John").Value,
            Lastname.Create("Doe").Value,
            email,
            PhoneNumber.Create("+33123456789").Value,
            "hashedpassword",
            UserProfileType.Customer
        );

        user.AddressId = address.Id;

        Context.Addresses.Add(address);
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(email, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Address.Should().NotBeNull();
        result.Address!.StreetNumber.Value.Should().Be("123");
        result.Address!.StreetName.Value.Should().Be("Test Street");
    }
}