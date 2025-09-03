using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetBoarding_Domain.Pets;
using PetBoarding_Domain.Users;

namespace PetBoarding_Persistence.Configurations;

internal sealed class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("Pets");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                petId => petId.Value,
                value => new PetId(value))
            .IsRequired();

        // Propriétés principales
        builder.Property(p => p.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.Breed)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Age)
            .IsRequired();

        builder.Property(p => p.Weight)
            .HasColumnType("decimal(5,2)");

        builder.Property(p => p.Color)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Gender)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.IsNeutered)
            .IsRequired();

        // Propriétés optionnelles
        builder.Property(p => p.MicrochipNumber)
            .HasMaxLength(50);

        builder.Property(p => p.MedicalNotes)
            .HasMaxLength(2000);

        builder.Property(p => p.SpecialNeeds)
            .HasMaxLength(2000);

        builder.Property(p => p.PhotoUrl)
            .HasMaxLength(500);

        // Relation avec User
        builder.Property(p => p.OwnerId)
            .HasConversion(
                ownerId => ownerId.Value,
                value => new UserId(value))
            .IsRequired();

        builder.HasOne(p => p.Owner)
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Contact d'urgence (value object intégré)
        builder.OwnsOne(p => p.EmergencyContact, emergencyContactBuilder =>
        {
            emergencyContactBuilder.Property(ec => ec.Name)
                .HasColumnName("EmergencyContactName")
                .HasMaxLength(100);

            emergencyContactBuilder.Property(ec => ec.Phone)
                .HasColumnName("EmergencyContactPhone")
                .HasMaxLength(20);

            emergencyContactBuilder.Property(ec => ec.Relationship)
                .HasColumnName("EmergencyContactRelationship")
                .HasMaxLength(50);
        });

        // Propriétés d'audit
        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        // ===== PERFORMANCE INDEXES =====
        // Index composite pour les animaux par propriétaire et type
        builder.HasIndex(p => new { p.OwnerId, p.Type })
            .HasDatabaseName("idx_pets_owner_type");

        // Index pour recherches par propriétaire seul
        builder.HasIndex(p => p.OwnerId)
            .HasDatabaseName("idx_pets_owner_id");

        // Index pour recherches par nom
        builder.HasIndex(p => p.Name)
            .HasDatabaseName("idx_pets_name");
    }
}