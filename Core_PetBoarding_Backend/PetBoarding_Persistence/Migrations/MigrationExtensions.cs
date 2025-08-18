using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PetBoarding_Persistence.Migrations
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
            
            ApplyMigrationsWithRetry(dbContext, logger);
        }

        private static void ApplyMigrationsWithRetry(ApplicationDbContext dbContext, ILogger logger, int maxRetries = 10)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    logger.LogInformation("Attempting to apply database migrations (attempt {Attempt}/{MaxRetries})", i + 1, maxRetries);
                    dbContext.Database.Migrate();
                    logger.LogInformation("Database migrations applied successfully");
                    return;
                }
                catch (Exception ex) when (i < maxRetries - 1)
                {
                    logger.LogWarning(ex, "Failed to apply migrations on attempt {Attempt}/{MaxRetries}. Retrying in {Delay} seconds...", 
                        i + 1, maxRetries, 5);
                    Thread.Sleep(5000); // Wait 5 seconds before retry
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to apply migrations after {MaxRetries} attempts", maxRetries);
                    throw;
                }
            }
        }
    }
}
