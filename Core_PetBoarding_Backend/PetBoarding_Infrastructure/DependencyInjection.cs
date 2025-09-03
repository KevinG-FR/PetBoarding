using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enyim.Caching.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Infrastructure.Caching;

namespace PetBoarding_Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

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
}
