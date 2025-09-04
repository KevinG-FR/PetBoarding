using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enyim.Caching.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Infrastructure.Caching;
using MassTransit;
using Microsoft.Extensions.Configuration;
using PetBoarding_Infrastructure.Events.Configuration;
using PetBoarding_Infrastructure.Events.Consumers;

namespace PetBoarding_Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        
        services.AddMassTransit(configuration);
        
        return services;
    }

    public static IServiceCollection AddMemcachedCache(this IServiceCollection services, string connectionString)
    {
        services.AddEnyimMemcached(options =>
        {
            // Parse la connection string pour séparer l'adresse et le port
            var parts = connectionString.Split(':');
            var address = parts[0];
            var port = parts.Length > 1 && int.TryParse(parts[1], out var parsedPort) ? parsedPort : 11211;
            
            options.Servers.Add(new Server { Address = address, Port = port });
        });

        services.AddScoped<ICacheService, MemcachedService>();

        return services;
    }

    private static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqSettings = configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>();
        var uriRabbitMq = new Uri($"rabbitmq://{rabbitMqSettings!.Host}:{rabbitMqSettings.Port}{rabbitMqSettings.VirtualHost}");

        services.AddMassTransit(x =>
        {
            // Register consumers
            x.AddConsumer<UserRegisteredEventConsumer>();
            x.AddConsumer<UserProfileUpdatedEventConsumer>();
            x.AddConsumer<PetRegisteredEventConsumer>();
            x.AddConsumer<ReservationCreatedEventConsumer>();
            x.AddConsumer<ReservationStatusChangedEventConsumer>();
            x.AddConsumer<PaymentProcessedEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(uriRabbitMq, h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
