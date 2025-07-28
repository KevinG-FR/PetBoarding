using System.Reflection;

using FluentAssertions;

using NetArchTest.Rules;

using PetBoarding_Domain.Abstractions;

namespace ArchitectureTests;

public class LayerTests
{
    private static readonly Assembly DomainAssembly = typeof(Entity<>).Assembly;
    private static readonly string ApplicationAssemblyName = "PetBoarding_Application";
    private static readonly string InfrastructureAssemblyName = "PetBoarding_Infrastructure";

    [Fact]
    public void Domain_Should_NotHaveDependencyOnApplication()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApplicationAssemblyName)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_Should_NotHaveDependencyOnInfrastructure()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssemblyName)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_Should_NotHaveDependencyOnApplication()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssemblyName)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
