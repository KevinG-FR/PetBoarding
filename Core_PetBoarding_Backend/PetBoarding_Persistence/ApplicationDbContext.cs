using Microsoft.EntityFrameworkCore;

using PetBoarding_Domain.Abstractions;
using PetBoarding_Domain.Addresses;
using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Payments;
using PetBoarding_Domain.Pets;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;
using PetBoarding_Domain.Reservations;
using PetBoarding_Domain.Users;
using PetBoarding_Persistence.Seeders;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace PetBoarding_Persistence
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationSlot> ReservationSlots { get; set; }
        public DbSet<Prestation> Prestations { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Planning> Plannings { get; set; }
        public DbSet<AvailableSlot> AvailableSlots { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IServiceProvider serviceProvider)
            : base(options) 
        { 
            _serviceProvider = serviceProvider;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.HasDefaultSchema("PetBoarding");

            // Seed des données initiales - supprimé pour utiliser un script SQL séparé
            // PrestationSeeder.SeedPrestations(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            
            var entitiesWithDomainEvents = ChangeTracker.Entries()
                .Where(e => e.Entity is IEntityWithDomainEvents entity && entity.GetDomainEvents().Count != 0)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            if (entitiesWithDomainEvents.Count != 0)
            {
                await PublishDomainEventsAsync(entitiesWithDomainEvents, cancellationToken);
            }            

            return result;
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries<IAuditableEntity>()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                entry.Entity.UpdateTimestamp();
            }
        }

        private async Task PublishDomainEventsAsync(List<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> entitiesWithDomainEvents, CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            logger.LogInformation("PublishDomainEventsAsync called with {EntitiesCount} entities with domain events", entitiesWithDomainEvents.Count);

            foreach (var entry in entitiesWithDomainEvents)
            {
                if (entry.Entity is not IEntityWithDomainEvents entity) continue;
                
                var domainEvents = entity.GetDomainEvents().ToList();
                logger.LogInformation("Entity {EntityType} has {EventsCount} domain events", entry.Entity.GetType().Name, domainEvents.Count);

                foreach (var domainEvent in domainEvents)
                {
                    logger.LogInformation("Publishing domain event {EventType} with EventId {EventId}", domainEvent.GetType().Name, domainEvent.EventId);
                    
                    // Publication vers MassTransit/RabbitMQ avec le type concret
                    await publishEndpoint.Publish(domainEvent, domainEvent.GetType(), cancellationToken);
                    
                    logger.LogInformation("Successfully published domain event {EventType} with EventId {EventId}", domainEvent.GetType().Name, domainEvent.EventId);
                }

                entity.ClearDomainEvents();
            }
        }
    }
}