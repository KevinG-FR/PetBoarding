namespace PetBoarding_Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Reservations;
using PetBoarding_Persistence.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuration de la base de données
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Enregistrement des repositories
        services.AddScoped<IReservationRepository, ReservationRepository>();

        return services;
    }
}
