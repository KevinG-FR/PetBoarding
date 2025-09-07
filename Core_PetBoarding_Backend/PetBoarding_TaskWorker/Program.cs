using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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

// Configuration du logging par défaut avec OpenTelemetry
builder.Logging.AddConsole();
builder.Logging.AddOpenTelemetry(logging =>
{
    var serviceName = Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME") ?? "PetBoarding.TaskWorker";
    logging.SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService(serviceName, serviceVersion: "1.0.0"));
    logging.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri(Environment.GetEnvironmentVariable("SIGNOZ_OTEL_ENDPOINT") ?? "http://localhost:4317");
    });
});

// Configuration OpenTelemetry
var serviceName = Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME") ?? "PetBoarding.TaskWorker";
var serviceVersion = "1.0.0";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
    .WithTracing(tracing => tracing
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation(options =>
        {
            options.SetDbStatementForText = true;
            options.SetDbStatementForStoredProcedure = true;
        })
        .AddSource("Quartz.Core") // Pour tracer les jobs Quartz
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(Environment.GetEnvironmentVariable("SIGNOZ_OTEL_ENDPOINT") ?? "http://localhost:4317");
        }))
    .WithMetrics(metrics => metrics
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(Environment.GetEnvironmentVariable("SIGNOZ_OTEL_ENDPOINT") ?? "http://localhost:4317");
        }));

// Add layers
builder.Services
    .AddApplicationCore()
    .AddInfrastructure(builder.Configuration)
    .AddMemcachedCache(builder.Configuration.GetConnectionString("Memcached") ?? "memcached")
    .AddPersistence();

// Configure Quartz
builder.Services.AddQuartz(q =>
{
    // Configuration de la base de données PostgreSQL pour la persistance des jobs
    var connectionString = builder.Configuration.GetConnectionString("Database") ?? 
        throw new InvalidOperationException("Database connection string not found");
        
    q.UsePersistentStore(s =>
    {
        s.UseProperties = true;
        s.UsePostgres(connectionString);
        s.UseClustering();
    });
    
    // Explicit configuration for serializer
    q.SetProperty("quartz.serializer.type", "json");

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
