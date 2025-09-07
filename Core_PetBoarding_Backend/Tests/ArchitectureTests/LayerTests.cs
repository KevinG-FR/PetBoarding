using FluentAssertions;
using NetArchTest.Rules;
using PetBoarding_Api.Mappers.Users;
using PetBoarding_Application.Core.Users.GetAllUsers;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Infrastructure.Caching;
using PetBoarding_Persistence;
using System.Reflection;

namespace ArchitectureTests;

public class LayerTests
{
    private static readonly Assembly DomainAssembly = typeof(Entity<>).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(GetAllUsersQueryHandler).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(MemcachedService).Assembly;
    private static readonly Assembly PersistenceAssembly = typeof(UnitOfWork).Assembly;
    private static readonly Assembly ApiAssembly = typeof(UserResponseMapper).Assembly;

    private static readonly string ApplicationAssemblyName = "PetBoarding_Application";
    private static readonly string InfrastructureAssemblyName = "PetBoarding_Infrastructure";
    private static readonly string PersistenceAssemblyName = "PetBoarding_Persistence";
    private static readonly string ApiAssemblyName = "PetBoarding_Api";

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
    public void Domain_Should_NotHaveDependencyOnPersistence()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(PersistenceAssemblyName)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_Should_NotHaveDependencyOnApi()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApiAssemblyName)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_OnlyDependOnDomain()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOnAny(InfrastructureAssemblyName, PersistenceAssemblyName, ApiAssemblyName)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_NotHaveDependencyOnInfrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssemblyName)
            .GetResult();
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_Should_NotHaveDependencyOnPersistence()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOn(PersistenceAssemblyName)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_Should_NotHaveDependencyOnApi()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOn(ApiAssemblyName)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Persistence_Should_OnlyDependOnDomainAndApplication()
    {
        var result = Types.InAssembly(PersistenceAssembly)
            .Should()
            .NotHaveDependencyOnAny(InfrastructureAssemblyName, ApiAssemblyName)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_Should_NotHaveExternalDependencies()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOnAny(
                "System.Net",
                "System.Net.Http",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "System.Data",
                "Npgsql")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Entities_Should_InheritFromEntityBase()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .ResideInNamespace("PetBoarding_Domain.Entities")
            .And()
            .AreNotAbstract()
            .Should()
            .Inherit(typeof(Entity<>))
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ValueObjects_Should_BeInValueObjectsNamespace()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .HaveNameEndingWith("Id")
            .Or()
            .HaveNameEndingWith("Email")
            .Should()
            .ResideInNamespaceStartingWith("PetBoarding_Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void CommandHandlers_Should_FollowNamingConvention()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("CommandHandler")
            .Should()
            .BeClasses()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void QueryHandlers_Should_FollowNamingConvention()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("QueryHandler")
            .Should()
            .BeClasses()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void RepositoryInterfaces_Should_BeInDomain()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .HaveNameEndingWith("Repository")
            .And()
            .AreInterfaces()
            .Should()
            .ResideInNamespaceStartingWith("PetBoarding_Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void RepositoryImplementations_Should_BeInPersistence()
    {
        var result = Types.InAssembly(PersistenceAssembly)
            .That()
            .HaveNameEndingWith("Repository")
            .And()
            .AreNotInterfaces()
            .Should()
            .ResideInNamespace("PetBoarding_Persistence.Repositories")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Commands_Should_BeInApplication()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Command")
            .Should()
            .ResideInNamespaceStartingWith("PetBoarding_Application")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Queries_Should_BeInApplication()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Query")
            .Should()
            .ResideInNamespaceStartingWith("PetBoarding_Application")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Endpoints_Should_BeInApi()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .HaveNameEndingWith("Endpoints")
            .Should()
            .ResideInNamespaceStartingWith("PetBoarding_Api.Endpoints")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Commands_Should_BeClasses()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Command")
            .And()
            .AreNotInterfaces()
            .Should()
            .BeClasses()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Queries_Should_BeClasses()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Query")
            .And()
            .AreNotInterfaces()
            .Should()
            .BeClasses()
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
