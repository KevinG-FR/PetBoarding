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

        // ===== PERFORMANCE INDEXES =====
        // Index composite pour les filtres les plus fréquents
        builder.HasIndex(p => new { p.EstDisponible, p.CategorieAnimal })
            .HasDatabaseName("idx_prestations_disponible_categorie");

        // Index pour les recherches par catégorie seule
        builder.HasIndex(p => new { p.CategorieAnimal, p.Libelle })
            .HasDatabaseName("idx_prestations_categorie_libelle");

        // Index pour les tris chronologiques (ordre descendant)
        builder.HasIndex(p => p.DateCreation)
            .IsDescending()
            .HasDatabaseName("idx_prestations_date_creation");

        // Index partiel pour la recherche de prestations disponibles
        builder.HasIndex(p => p.EstDisponible)
            .HasDatabaseName("idx_prestations_disponible")
            .HasFilter("\"EstDisponible\" = true");
    }
}
