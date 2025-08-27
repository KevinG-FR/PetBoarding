namespace PetBoarding_Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetBoarding_Domain.Planning;
using PetBoarding_Domain.Prestations;
using System.Text.Json;

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

        // Configuration de la collection de créneaux en JSON
        builder.Property(p => p.Creneaux)
            .HasConversion(
                creneaux => JsonSerializer.Serialize(
                    creneaux.Select(c => new
                    {
                        Date = c.Date,
                        CapaciteMax = c.MaxCapacity,
                        CapaciteReservee = c.CapaciteReservee
                    }),
                    (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<CreneauData[]>(json, (JsonSerializerOptions?)null)?
                    .Select(data => new AvailableSlot(data.Date, data.CapaciteMax, data.CapaciteReservee))
                    .ToList() ?? new List<AvailableSlot>())
            .HasColumnType("json");

        // Index sur PrestationId pour optimiser les recherches
        builder.HasIndex(p => p.PrestationId)
            .IsUnique()
            .HasDatabaseName("IX_Plannings_PrestationId");

        // Index sur EstActif pour optimiser les recherches des plannings actifs
        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("IX_Plannings_EstActif");
    }

    // Classe helper pour la sérialisation JSON
    private sealed class CreneauData
    {
        public DateTime Date { get; set; }
        public int CapaciteMax { get; set; }
        public int CapaciteReservee { get; set; }
    }
}