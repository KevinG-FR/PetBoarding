using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PetBoarding_Domain.Addresses;
using PetBoarding_Domain.Users;
using System.IO;

namespace PersistenceIntegrationTests.Configurations;

public class UserConfigurationTests : PostgreSqlTestBase
{
    [Fact]
    public void UserConfiguration_Should_ConfigureTableName()
    {
        // Arrange & Act
        var entityType = Context.Model.FindEntityType(typeof(User));

        // Assert
        entityType.Should().NotBeNull();
        entityType!.GetTableName().Should().Be("Users");
        entityType.GetSchema().Should().Be("PetBoarding");
    }

    [Fact]
    public void UserConfiguration_Should_ConfigurePrimaryKey()
    {
        // Arrange & Act
        var entityType = Context.Model.FindEntityType(typeof(User));
        var primaryKey = entityType!.FindPrimaryKey();

        // Assert
        primaryKey.Should().NotBeNull();
        primaryKey!.Properties.Should().HaveCount(1);
        primaryKey.Properties.First().Name.Should().Be("Id");
    }

    [Fact]
    public void UserConfiguration_Should_ConfigureValueObjectConversions()
    {
        // Arrange        
        var user = User.Create(
            Firstname.Create("John").Value,
            Lastname.Create("Doe").Value,
            Email.Create("john.doe@test.com").Value,
            PhoneNumber.Create("+33123456789").Value,
            "hashedpassword",
            UserProfileType.Customer
        );

        // Act
        Context.Users.Add(user);
        Context.SaveChanges();

        // Assert - Vérifier que les conversions fonctionnent
        var userFromDb = Context.Users.First(u => u.Id == user.Id);
        
        userFromDb.Should().NotBeNull();
        userFromDb.Id.Should().Be(user.Id);
        userFromDb.Firstname.Value.Should().Be("John");
        userFromDb.Lastname.Value.Should().Be("Doe");
        userFromDb.Email.Value.Should().Be("john.doe@test.com");
        userFromDb.PhoneNumber.Value.Should().Be("+33123456789");
        userFromDb.AddressId.Should().Be(user.AddressId);
    }

    [Fact]
    public void UserConfiguration_Should_ConfigureEnumConversions()
    {
        // Arrange        
        var user = User.Create(
            Firstname.Create("John").Value,
            Lastname.Create("Doe").Value,
            Email.Create("john.doe@test.com").Value,
            PhoneNumber.Create("+33123456789").Value,
            "hashedpassword",
            UserProfileType.Customer
        );

        // Act
        Context.Users.Add(user);
        Context.SaveChanges();

        // Assert
        var userFromDb = Context.Users.First(u => u.Id == user.Id);
        
        userFromDb.Status.Should().Be(UserStatus.Created);
        userFromDb.ProfileType.Should().Be(UserProfileType.Customer);
    }

    [Fact]
    public void UserConfiguration_Should_ConfigureRequiredFields()
    {
        // Arrange & Act
        var entityType = Context.Model.FindEntityType(typeof(User));

        // Assert
        var createdAtProperty = entityType!.FindProperty(nameof(User.CreatedAt));
        var updatedAtProperty = entityType.FindProperty(nameof(User.UpdatedAt));

        createdAtProperty.Should().NotBeNull();
        createdAtProperty!.IsNullable.Should().BeFalse();

        updatedAtProperty.Should().NotBeNull();
        updatedAtProperty!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void UserConfiguration_Should_ConfigureAddressRelationship()
    {
        // Arrange & Act
        var entityType = Context.Model.FindEntityType(typeof(User));
        var addressNavigation = entityType!.FindNavigation(nameof(User.Address));
        var addressIdProperty = entityType.FindProperty(nameof(User.AddressId));

        // Assert
        addressNavigation.Should().NotBeNull();
        addressNavigation!.ForeignKey.DeleteBehavior.Should().Be(DeleteBehavior.SetNull);

        addressIdProperty.Should().NotBeNull();
        addressIdProperty!.IsNullable.Should().BeTrue(); // AddressId est nullable
    }

    [Fact]
    public void UserConfiguration_Should_ConfigureIndexes()
    {
        // Arrange & Act
        var entityType = Context.Model.FindEntityType(typeof(User));
        var indexes = entityType!.GetIndexes().ToList();

        // Assert
        indexes.Should().HaveCountGreaterThan(0);

        // Index composite pour l'authentification
        var authIndex = indexes.FirstOrDefault(i => 
            i.Properties.Count == 2 && 
            i.Properties.Any(p => p.Name == nameof(User.Email)) &&
            i.Properties.Any(p => p.Name == nameof(User.PasswordHash))
        );
        authIndex.Should().NotBeNull();
        authIndex!.GetDatabaseName().Should().Be("idx_users_email_password");

        // Index pour les recherches par email
        var emailIndex = indexes.FirstOrDefault(i => 
            i.Properties.Count == 1 && 
            i.Properties.First().Name == nameof(User.Email)
        );
        emailIndex.Should().NotBeNull();
        emailIndex!.GetDatabaseName().Should().Be("idx_users_email");

        // Index pour les timestamps d'audit
        var createdAtIndex = indexes.FirstOrDefault(i => 
            i.Properties.Count == 1 && 
            i.Properties.First().Name == nameof(User.CreatedAt)
        );
        createdAtIndex.Should().NotBeNull();
        createdAtIndex!.GetDatabaseName().Should().Be("idx_users_created_at");
    }        

    [Fact]
    public void UserConfiguration_Should_ConfigureValueObjectProperties()
    {
        // Arrange & Act
        var entityType = Context.Model.FindEntityType(typeof(User));
        
        // Assert
        var firstnameProperty = entityType!.FindProperty(nameof(User.Firstname));
        var lastnameProperty = entityType.FindProperty(nameof(User.Lastname));
        var emailProperty = entityType.FindProperty(nameof(User.Email));
        var phoneProperty = entityType.FindProperty(nameof(User.PhoneNumber));

        firstnameProperty.Should().NotBeNull();
        lastnameProperty.Should().NotBeNull();
        emailProperty.Should().NotBeNull();
        phoneProperty.Should().NotBeNull();

        // Ces propriétés devraient être configurées avec leurs conversions
        firstnameProperty!.GetValueConverter().Should().NotBeNull();
        lastnameProperty!.GetValueConverter().Should().NotBeNull();
        emailProperty!.GetValueConverter().Should().NotBeNull();
        phoneProperty!.GetValueConverter().Should().NotBeNull();
    }
}