using System.Reflection;

using FluentAssertions;

using NetArchTest.Rules;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Addresses;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Payments;
using PetBoarding_Domain.Pets;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;
using PetBoarding_Domain.Primitives;
using PetBoarding_Domain.Reservations;
using PetBoarding_Domain.Users;

namespace ArchitectureTests;

public class DomainTests
{
    private static readonly Assembly DomainAssembly = typeof(Entity<>).Assembly;

    [Fact]
    public void DomainEvents_Should_BeSealed()
    {
        var result = Types.InAssembly(DomainAssembly)
        .That()
        .ImplementInterface(typeof(IDomainEvent))
        .And().AreNotAbstract()
        .Should()
        .BeSealed()
        .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainEvents_Should_HaveDomainEventPostFix()
    {
        var result = Types.InAssembly(DomainAssembly)
        .That()
        .ImplementInterface(typeof(IDomainEvent))
        .Should()
        .HaveNameEndingWith("DomainEvent")
        .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ConcreteEntities_Should_InheritFromEntityBaseClasses()
    {
        // Teste que toutes les entités concrètes héritent bien d'une des classes de base Entity
        var concreteEntities = new[]
        {
            nameof(User), nameof(Address), nameof(Pet), nameof(Prestation), nameof(Reservation), nameof(ReservationSlot),
            nameof(Planning), nameof(AvailableSlot), nameof(Basket), nameof(BasketItem), nameof(Payment)
        };

        var result = Types.InAssembly(DomainAssembly)
        .That()
        .HaveNameMatching(string.Join("|", concreteEntities.Select(e => $"^{e}$")))
        .Should()
        .Inherit(typeof(Entity<>))
        .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Entities_Should_BeInCorrectNamespace()
    {
        var result = Types.InAssembly(DomainAssembly)
        .That()
        .Inherit(typeof(Entity<>))
        .Should()
        .ResideInNamespaceMatching("PetBoarding_Domain\\.[A-Za-z]+$")
        .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Entities_Should_NotReferenceInfrastructure()
    {
        var result = Types.InAssembly(DomainAssembly)
        .That()
        .Inherit(typeof(Entity<>))
        .Should()
        .NotHaveDependencyOn("PetBoarding_Infrastructure")
        .And()
        .NotHaveDependencyOn("PetBoarding_Persistence")
        .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ConcreteValueObjects_Should_InheritFromValueObject()
    {
        // Teste que les Value Objects connus héritent bien de ValueObject
        var valueObjects = new[]
        {
            nameof(Email), nameof(Firstname), nameof(Lastname), nameof(PhoneNumber), 
            nameof(StreetNumber), nameof(StreetName), nameof(City), nameof(PostalCode), 
            nameof(Country), nameof(Complement), nameof(EmergencyContact)
        };

        var result = Types.InAssembly(DomainAssembly)
        .That()
        .HaveNameMatching(string.Join("|", valueObjects.Select(vo => $"^{vo}$")))
        .Should()
        .Inherit(typeof(ValueObject))
        .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ValueObjects_Should_BeImmutable()
    {
        var valueObjectTypes = Types.InAssembly(DomainAssembly)
        .That()
        .Inherit(typeof(ValueObject))
        .GetTypes();

        foreach (var type in valueObjectTypes)
        {
            var writableProperties = type.GetProperties()
                .Where(p => p.CanWrite && p.SetMethod?.IsPublic == true)
                .ToList();

            writableProperties.Should().BeEmpty($"{type.Name} should have only readonly properties");
        }
    }

    [Fact]
    public void ValueObjects_Should_ImplementGetAtomicValues()
    {
        var valueObjectTypes = Types.InAssembly(DomainAssembly)
        .That()
        .Inherit(typeof(ValueObject))
        .GetTypes();

        foreach (var type in valueObjectTypes)
        {
            var method = type.GetMethod("GetAtomicValues");
            method.Should().NotBeNull($"{type.Name} should implement GetAtomicValues method");
            method!.IsAbstract.Should().BeFalse($"{type.Name} should implement GetAtomicValues method (not abstract)");
        }
    }

    [Fact]
    public void RepositoryInterfaces_Should_InheritFromIBaseRepository()
    {
        var result = Types.InAssembly(DomainAssembly)
        .That()
        .AreInterfaces()
        .And().HaveNameEndingWith("Repository")
        .And().DoNotHaveName("IBaseRepository")
        .Should()
        .ImplementInterface(typeof(IBaseRepository<,>))
        .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void RepositoryInterfaces_Should_BeInCorrectNamespace()
    {
        var result = Types.InAssembly(DomainAssembly)
        .That()
        .AreInterfaces()
        .And().HaveNameEndingWith("Repository")
        .And().DoNotHaveName("IBaseRepository")
        .Should()
        .ResideInNamespaceMatching("PetBoarding_Domain\\.[A-Za-z]+$")
        .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_Should_NotHaveRepositoryImplementations()
    {
        var repositoryImplementations = Types.InAssembly(DomainAssembly)
        .That()
        .AreClasses()
        .And().HaveNameEndingWith("Repository")
        .GetTypes();

        repositoryImplementations.Should().BeEmpty("Domain should not contain repository implementations");
    }

    [Fact]
    public void EntityIdentifiers_Should_InheritFromEntityIdentifier()
    {
        var result = Types.InAssembly(DomainAssembly)
        .That()
        .HaveNameEndingWith("Id")
        .And().AreNotAbstract()
        .And().DoNotHaveName("EntityIdentifier")
        .Should()
        .Inherit(typeof(EntityIdentifier))
        .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void EntityIdentifiers_Should_FollowNamingPattern()
    {
        var entityIdentifierTypes = Types.InAssembly(DomainAssembly)
        .That()
        .Inherit(typeof(EntityIdentifier))
        .And().DoNotHaveName("EntityIdentifier")
        .GetTypes();

        foreach (var type in entityIdentifierTypes)
        {
            // Vérifie que le nom se termine par "Id" et commence par une majuscule
            type.Name.Should().EndWith("Id", $"{type.Name} should end with 'Id'");
            type.Name.Should().MatchRegex("^[A-Z][a-zA-Z]*Id$", $"{type.Name} should follow PascalCase naming pattern ending with 'Id'");
            
            // Vérifie que c'est un record
            type.IsValueType.Should().BeFalse($"{type.Name} should be a record (reference type)");
            var baseType = type.BaseType;
            baseType.Should().Be(typeof(EntityIdentifier), $"{type.Name} should directly inherit from EntityIdentifier");
        }
    }    
}
