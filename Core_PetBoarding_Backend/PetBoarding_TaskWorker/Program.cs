using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PetBoarding_Application;
using PetBoarding_Infrastructure;
using PetBoarding_Persistence;
using PetBoarding_TaskWorker.Jobs;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);

// Configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Logging
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

// Add layers
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPersistence();

// Configure Quartz
builder.Services.AddQuartz(q =>
{
    // Configuration de la base de données PostgreSQL pour la persistance des jobs
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
        throw new InvalidOperationException("DefaultConnection string not found");
        
    q.UsePersistentStore(s =>
    {
        s.UseProperties = true;
        s.UsePostgres(connectionString);
        s.UseClustering();
    });

    // Job de nettoyage des paniers expirés
    var cleanBasketsJobKey = new JobKey("CleanExpiredBasketsJob");
    q.AddJob<CleanExpiredBasketsJob>(opts => opts.WithIdentity(cleanBasketsJobKey));
    
    var cleanBasketsInterval = builder.Configuration.GetValue<int>("TaskWorker:ExpiredBasketCleanupIntervalMinutes", 10);
    q.AddTrigger(opts => opts
        .ForJob(cleanBasketsJobKey)
        .WithIdentity("CleanExpiredBasketsJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(cleanBasketsInterval)
            .RepeatForever())
        .StartNow());

    // Job de traitement des réservations expirées
    var processReservationsJobKey = new JobKey("ProcessExpiredReservationsJob");
    q.AddJob<ProcessExpiredReservationsJob>(opts => opts.WithIdentity(processReservationsJobKey));
    
    var processReservationsInterval = builder.Configuration.GetValue<int>("TaskWorker:ExpiredReservationProcessingIntervalMinutes", 15);
    q.AddTrigger(opts => opts
        .ForJob(processReservationsJobKey)
        .WithIdentity("ProcessExpiredReservationsJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(processReservationsInterval)
            .RepeatForever())
        .StartNow());
});

// Add Quartz hosted service
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var migrationLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        migrationLogger.LogInformation("Applying database migrations...");
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
        migrationLogger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        migrationLogger.LogError(ex, "Failed to apply database migrations");
        throw;
    }
}

// Start the application
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("PetBoarding TaskWorker starting...");

try
{
    await app.RunAsync();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "PetBoarding TaskWorker terminated unexpectedly");
    throw;
}
finally
{
    logger.LogInformation("PetBoarding TaskWorker stopped");
}
