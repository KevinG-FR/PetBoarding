using System.Reflection;

using FluentAssertions;

using NetArchTest.Rules;

using PetBoarding_Domain.Abstractions;

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
    public void Entities_Should_HavePrivateParameterlessConstructor()
    {
        var entityTypes = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(Entity<>))
            .GetTypes();

        var failingTypes = new List<Type>();

        foreach (var entityType in entityTypes)
        {
            var constructors = entityType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

            if (constructors.Any(c => c.IsPrivate && !c.GetParameters().Any()))
                failingTypes.Add(entityType);
        }

        failingTypes.Should().BeEmpty();
    }
}
