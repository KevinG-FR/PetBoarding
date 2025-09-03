namespace PetBoarding_Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PetBoarding_Domain.Reservations;

internal sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasConversion(
                reservationId => reservationId.Value,
                value => new ReservationId(value))
            .ValueGeneratedNever();

        builder.Property(r => r.UserId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.AnimalId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.AnimalName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.ServiceId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.StartDate)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(r => r.EndDate)
            .HasColumnType("date");

        builder.Property(r => r.Comments)
            .HasMaxLength(1000);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(r => r.UpdatedAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(r => r.TotalPrice)
            .HasColumnType("decimal(10,2)");

        builder.Property(r => r.PaidAt)
            .HasColumnType("timestamp with time zone");

        // ===== PERFORMANCE INDEXES =====
        // Index composite le plus critique (requêtes utilisateur)
        builder.HasIndex(r => new { r.UserId, r.CreatedAt })
            .HasDatabaseName("idx_reservations_userid_createdat")
            .IsDescending(false, true); // UserId ASC, CreatedAt DESC

        // Index composite pour recherches par service
        builder.HasIndex(r => new { r.ServiceId, r.CreatedAt })
            .HasDatabaseName("idx_reservations_serviceid_createdat")
            .IsDescending(false, true); // ServiceId ASC, CreatedAt DESC

        // Index composite pour les filtres de statut et dates
        builder.HasIndex(r => new { r.Status, r.StartDate })
            .HasDatabaseName("idx_reservations_status_startdate");

        // Index pour les recherches par plage de dates
        builder.HasIndex(r => new { r.StartDate, r.EndDate })
            .HasDatabaseName("idx_reservations_date_range");

        // Index partiel pour les réservations affichées à l'utilisateur
        builder.HasIndex(r => new { r.UserId, r.Status, r.CreatedAt })
            .HasDatabaseName("idx_reservations_user_displayed")
            .HasFilter("\"Status\" IN ('Validated', 'InProgress', 'Completed')")
            .IsDescending(false, false, true); // UserId ASC, Status ASC, CreatedAt DESC
    }
}
