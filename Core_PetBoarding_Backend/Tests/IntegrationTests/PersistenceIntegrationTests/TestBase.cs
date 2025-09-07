using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MassTransit;
using Moq;
using PetBoarding_Persistence;

namespace PersistenceIntegrationTests;

public abstract class TestBase : IDisposable
{
    protected ApplicationDbContext Context { get; private set; }
    protected Mock<IPublishEndpoint> MockPublishEndpoint { get; private set; }
    private IServiceProvider _localServiceProvider;

    protected TestBase()
    {
        MockPublishEndpoint = new Mock<IPublishEndpoint>();
        Context = CreateInMemoryContext();
    }

    private IServiceProvider CreateLocalServiceProvider()
    {
        var services = new ServiceCollection();
        
        // Ajouter uniquement les services nécessaires pour les mocks
        services.AddSingleton(MockPublishEndpoint.Object);
        
        // Ajouter un mock logger
        var mockLogger = new Mock<ILogger<ApplicationDbContext>>();
        services.AddSingleton(mockLogger.Object);
        
        return services.BuildServiceProvider();
    }

    private ApplicationDbContext CreateInMemoryContext()
    {
        // Créer un ServiceProvider local spécifiquement pour ce contexte
        _localServiceProvider = CreateLocalServiceProvider();
        
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options, _localServiceProvider);
        
        // Assurer que la base de données est créée
        context.Database.EnsureCreated();
        
        return context;
    }

    protected virtual void SeedData()
    {
        // Override this method in derived classes to add test data
    }

    public virtual void Dispose()
    {
        Context?.Dispose();
        
        if (_localServiceProvider is IDisposable disposableServiceProvider)
        {
            disposableServiceProvider.Dispose();
        }
    }
}