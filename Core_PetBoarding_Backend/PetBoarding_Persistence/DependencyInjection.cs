namespace PetBoarding_Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Pets;
using PetBoarding_Domain.Prestations;
using PetBoarding_Domain.Reservations;
using PetBoarding_Domain.Users;
using PetBoarding_Persistence.Options;
using PetBoarding_Persistence.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services)
    {
        // Configuration de la base de données
        services.ConfigureOptions<DatabaseOptionsSetup>();

        services.AddDbContext<ApplicationDbContext>(
            (serviceProvider, dbContextOptionsBuilder) =>
            {
                var databaseOptions = serviceProvider.GetService<IOptions<DatabaseOptions>>()!.Value;

                dbContextOptionsBuilder.UseNpgsql(databaseOptions.ConnectionString, postgreAction =>
                {
                    postgreAction.CommandTimeout(databaseOptions.CommandTimeout);
                    postgreAction.EnableRetryOnFailure(databaseOptions.MaxRetry);
                });

                dbContextOptionsBuilder.EnableDetailedErrors(databaseOptions.EnableDetailedErrors);
                dbContextOptionsBuilder.EnableSensitiveDataLogging(databaseOptions.EnableSensitiveDataLogging);
            });

        // Enregistrement des repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IPrestationRepository, PrestationRepository>();
        services.AddScoped<IPetRepository, PetRepository>();

        // Enregistrement de l'UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
