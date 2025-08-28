namespace PetBoarding_Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;

public sealed class PlanningConfiguration : IEntityTypeConfiguration<Planning>
{
    public void Configure(EntityTypeBuilder<Planning> builder)
    {
        builder.ToTable("Plannings");

        // Configuration de la clé primaire
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => new PlanningId(value))
            .ValueGeneratedNever();

        // Configuration du PrestationId
        builder.Property(p => p.PrestationId)
            .HasConversion(
                id => id.Value,
                value => new PrestationId(value))
            .IsRequired();

        // Configuration des propriétés de base
        builder.Property(p => p.Label)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.DateCreation)
            .IsRequired();

        builder.Property(p => p.DateModification);

        // Index sur PrestationId pour optimiser les recherches
        builder.HasIndex(p => p.PrestationId)
            .IsUnique()
            .HasDatabaseName("IX_Plannings_PrestationId");

        // Index sur IsActive pour optimiser les recherches des plannings actifs
        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_Plannings_IsActive");

        // Index composite pour requêtes courantes
        builder.HasIndex(p => new { p.IsActive, p.PrestationId })
            .HasDatabaseName("IX_Plannings_Active_Prestation");

        // Configuration de la relation avec AvailableSlots
        builder.HasMany(p => p.Creneaux)
            .WithOne()
            .HasForeignKey(s => s.PlanningId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}