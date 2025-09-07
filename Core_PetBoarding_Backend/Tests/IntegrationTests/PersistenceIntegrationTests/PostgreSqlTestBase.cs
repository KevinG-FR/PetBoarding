using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MassTransit;
using Moq;
using PetBoarding_Persistence;
using Testcontainers.PostgreSql;
using Xunit;

namespace PersistenceIntegrationTests;

public abstract class PostgreSqlTestBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    protected ApplicationDbContext Context { get; private set; }
    protected Mock<IPublishEndpoint> MockPublishEndpoint { get; private set; }
    protected IServiceProvider ServiceProvider { get; private set; }

    protected PostgreSqlTestBase()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .Build();

        MockPublishEndpoint = new Mock<IPublishEndpoint>();
        ServiceProvider = SetupServiceProvider();
    }

    private IServiceProvider SetupServiceProvider()
    {
        var services = new ServiceCollection();
        
        // Ajouter les mocks comme services
        services.AddSingleton(MockPublishEndpoint.Object);
        
        // Ajouter un mock logger au lieu d'utiliser AddConsole
        var mockLogger = new Mock<ILogger<ApplicationDbContext>>();
        services.AddSingleton(mockLogger.Object);
        
        return services.BuildServiceProvider();
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        Context = await CreatePostgreSqlContextAsync();
        await SeedDataAsync();
    }

    private async Task<ApplicationDbContext> CreatePostgreSqlContextAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString())
            .Options;

        var context = new ApplicationDbContext(options, ServiceProvider);
        
        // Appliquer les migrations
        await context.Database.MigrateAsync();
        
        return context;
    }

    protected virtual async Task SeedDataAsync()
    {
        // Override this method in derived classes to add test data
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (Context != null)
        {
            await Context.DisposeAsync();
        }
        
        if (ServiceProvider is IAsyncDisposable asyncDisposableServiceProvider)
        {
            await asyncDisposableServiceProvider.DisposeAsync();
        }
        else if (ServiceProvider is IDisposable disposableServiceProvider)
        {
            disposableServiceProvider.Dispose();
        }
        
        if (_postgresContainer != null)
        {
            await _postgresContainer.DisposeAsync();
        }
    }
}