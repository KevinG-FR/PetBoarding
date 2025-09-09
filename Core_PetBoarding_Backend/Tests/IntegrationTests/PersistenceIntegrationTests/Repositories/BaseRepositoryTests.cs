using FluentAssertions;
using PersistenceIntegrationTests.TestHelpers;
using PetBoarding_Domain.Addresses;
using PetBoarding_Domain.Users;
using PetBoarding_Persistence.Repositories;

namespace PersistenceIntegrationTests.Repositories;

public class BaseRepositoryTests : PostgreSqlTestBase
{
    private UserRepository _repository => new UserRepository(Context);

    [Fact]
    public async Task GetByIdAsync_Should_ReturnEntity_When_EntityExists()
    {
        // Arrange
        var user = EntityTestFactory.CreateUser();

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Email.Value.Should().Be("john.doe@test.com");
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_When_EntityDoesNotExist()
    {
        // Arrange
        var nonExistentId = new UserId(Guid.NewGuid());

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetWithFilterAsync_Should_ReturnEntity_When_FilterMatches()
    {
        // Arrange
        var user = EntityTestFactory.CreateUser();

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetWithFilterAsync(u => u.Email == user.Email, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetAllAsync_Should_ReturnAllEntities()
    {
        // Arrange
        var users = new List<User>
        {
            User.Create(
                Firstname.Create("John").Value,
                Lastname.Create("Doe").Value,
                Email.Create("john.doe@test.com").Value,
                PhoneNumber.Create("+33123456789").Value,
                "hashedpassword",
                UserProfileType.Customer
            ),
            User.Create(
                Firstname.Create("Jane").Value,
                Lastname.Create("Smith").Value,
                Email.Create("jane.smith@test.com").Value,
                PhoneNumber.Create("+33987654321").Value,
                "hashedpassword2",
                UserProfileType.Customer
            )
        };

        Context.Users.AddRange(users);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(u => u.Email.Value == "john.doe@test.com");
        result.Should().Contain(u => u.Email.Value == "jane.smith@test.com");
    }

    [Fact]
    public async Task GetAllWithFilterAsync_Should_ReturnFilteredEntities()
    {
        // Arrange
        var users = new List<User>
        {
            User.Create(
                Firstname.Create("John").Value,
                Lastname.Create("Doe").Value,
                Email.Create("john.doe@test.com").Value,
                PhoneNumber.Create("+33123456789").Value,
                "hashedpassword",
                UserProfileType.Customer
            ),
            User.Create(
                Firstname.Create("Jane").Value,
                Lastname.Create("Doe").Value,
                Email.Create("jane.doe@test.com").Value,
                PhoneNumber.Create("+33987654321").Value,
                "hashedpassword2",
                UserProfileType.Customer
            )
        };

        Context.Users.AddRange(users);
        await Context.SaveChangesAsync();

        var lastname = Lastname.Create("Doe").Value;

        // Act
        var result = await _repository.GetAllWithFilterAsync(u => u.Lastname == lastname, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(u => u.Lastname.Value == "Doe");
    }

    [Fact]
    public async Task AddAsync_Should_AddEntity_And_ReturnAddedEntity()
    {
        // Arrange
        var user = EntityTestFactory.CreateUser();

        // Act
        var result = await _repository.AddAsync(user, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        
        var entityInDb = await Context.Users.FindAsync(user.Id);
        entityInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_Should_UpdateExistingEntity()
    {
        // Arrange
        var user = EntityTestFactory.CreateUser();

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var updatedEmail = Email.Create("john.updated@test.com").Value;
        user.Email = updatedEmail;

        // Act
        var result = await _repository.UpdateAsync(user, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(updatedEmail);
    }

    [Fact]
    public async Task UpdateAsync_Should_ReturnNull_When_EntityDoesNotExist()
    {
        // Arrange
        var nonExistentUser = EntityTestFactory.CreateUser();

        // Act
        var result = await _repository.UpdateAsync(nonExistentUser, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_DeleteEntity_And_ReturnDeletedCount()
    {
        // Arrange
        var user = EntityTestFactory.CreateUser();

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(user, CancellationToken.None);

        // Assert
        result.Should().Be(1);
        
        var entityInDb = await Context.Users.FindAsync(user.Id);
        entityInDb.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_Should_ReturnZero_When_EntityDoesNotExist()
    {
        // Arrange
        var nonExistentUser = User.Create(
            Firstname.Create("John").Value,
            Lastname.Create("Doe").Value,
            Email.Create("john.doe@test.com").Value,
            PhoneNumber.Create("+33123456789").Value,
            "hashedpassword",
            UserProfileType.Customer
        );

        // Act
        var result = await _repository.DeleteAsync(nonExistentUser, CancellationToken.None);

        // Assert
        result.Should().Be(0);
    }
}