using FluentAssertions;
using PersistenceIntegrationTests.TestHelpers;
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
        var address = EntityTestFactory.CreateAddress();

        var user = EntityTestFactory.CreateUser();

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
        var user = EntityTestFactory.CreateUser();

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.UserEmailAlreadyUsed(user.Email.Value, CancellationToken.None);

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
        const string passwordHash = "hashedpassword";

        var user = EntityTestFactory.CreateUser();

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserForAuthentification(user.Email.Value, passwordHash, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Value.Should().Be(user.Email.Value);
        result.PasswordHash.Should().Be(passwordHash);
    }

    [Fact]
    public async Task GetUserForAuthentification_Should_ReturnNull_When_EmailIsWrong()
    {
        // Arrange
        const string wrongEmail = "wrong@test.com";
        const string passwordHash = "hashedpassword";

        var user = EntityTestFactory.CreateUser();

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
        const string wrongPasswordHash = "wrongpassword";

        var user = EntityTestFactory.CreateUser();

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserForAuthentification(user.Email.Value, wrongPasswordHash, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserForAuthentification_Should_IncludeAddress()
    {
        // Arrange
        const string passwordHash = "hashedpassword";

        var address = EntityTestFactory.CreateAddress();

        var user = EntityTestFactory.CreateUser();

        user.AddressId = address.Id;

        Context.Addresses.Add(address);
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserForAuthentification(user.Email.Value, passwordHash, CancellationToken.None);

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
        var user = EntityTestFactory.CreateUser();

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(user.Email, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(user.Email);
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

        var address = EntityTestFactory.CreateAddress();

        var user = EntityTestFactory.CreateUser();

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