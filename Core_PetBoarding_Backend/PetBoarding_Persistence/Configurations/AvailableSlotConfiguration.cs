namespace PetBoarding_Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetBoarding_Domain.Planning;

public sealed class AvailableSlotConfiguration : IEntityTypeConfiguration<AvailableSlot>
{
    public void Configure(EntityTypeBuilder<AvailableSlot> builder)
    {
        builder.ToTable("AvailableSlots");

        // Configuration de la clé primaire
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => new AvailableSlotId(value))
            .ValueGeneratedNever();

        // Configuration de la FK vers Planning
        builder.Property(s => s.PlanningId)
            .HasConversion(
                id => id.Value,
                value => new PlanningId(value))
            .IsRequired();

        // Configuration des propriétés
        builder.Property(s => s.Date)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(s => s.MaxCapacity)
            .IsRequired();

        builder.Property(s => s.CapaciteReservee)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(s => s.ModifiedAt);

        // Relation avec Planning (pas de configuration explicite car PlanningId est déjà configuré)

        // Index composite pour unicité et performance
        builder.HasIndex(s => new { s.PlanningId, s.Date })
               .IsUnique()
               .HasDatabaseName("IX_AvailableSlots_Planning_Date");
               
        // Index pour requêtes par date
        builder.HasIndex(s => s.Date)
               .HasDatabaseName("IX_AvailableSlots_Date");

        // Index pour requêtes par capacité disponible
        builder.HasIndex(s => new { s.Date, s.MaxCapacity, s.CapaciteReservee })
               .HasDatabaseName("IX_AvailableSlots_Availability");
    }
}