namespace PetBoarding_Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetBoarding_Domain.Prestations;

internal sealed class PrestationConfiguration : IEntityTypeConfiguration<Prestation>
{
    public void Configure(EntityTypeBuilder<Prestation> builder)
    {
        builder.ToTable("Prestations");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                prestationId => prestationId.Value,
                value => new PrestationId(value))
            .IsRequired();

        builder.Property(p => p.Libelle)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.CategorieAnimal)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.Prix)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(p => p.DureeEnMinutes)
            .IsRequired();

        builder.Property(p => p.EstDisponible)
            .IsRequired();

        builder.Property(p => p.DateCreation)
            .IsRequired();

        builder.Property(p => p.DateModification);

        // Index pour optimiser les recherches
        builder.HasIndex(p => p.CategorieAnimal);
        builder.HasIndex(p => p.EstDisponible);
        builder.HasIndex(p => new { p.CategorieAnimal, p.EstDisponible });
    }
}
