namespace PetBoarding_Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PetBoarding_Domain.Reservations;

internal sealed class ReservationSlotConfiguration : IEntityTypeConfiguration<ReservationSlot>
{
    public void Configure(EntityTypeBuilder<ReservationSlot> builder)
    {
        builder.ToTable("ReservationSlots");

        builder.HasKey(rs => rs.Id);

        // Configuration de l'ID (Value Object)
        builder.Property(rs => rs.Id)
            .HasConversion(
                reservationSlotId => reservationSlotId.Value,
                value => new ReservationSlotId(value))
            .ValueGeneratedNever();

        // Configuration de ReservationId (Value Object)
        builder.Property(rs => rs.ReservationId)
            .HasConversion(
                reservationId => reservationId.Value,
                value => new ReservationId(value))
            .IsRequired()
            .HasMaxLength(36); // Guid as string

        // Configuration de l'AvailableSlotId
        builder.Property(rs => rs.AvailableSlotId)
            .IsRequired();

        // Configuration des dates (PostgreSQL compatible)
        builder.Property(rs => rs.ReservedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("NOW()");

        builder.Property(rs => rs.ReleasedAt)
            .HasColumnType("timestamp with time zone");

        // ===== PERFORMANCE INDEXES =====
        // Index composite pour les jointures (garde la contrainte unique)
        builder.HasIndex(rs => new { rs.ReservationId, rs.AvailableSlotId })
            .IsUnique()
            .HasDatabaseName("idx_reservation_slots_reservation_available");

        // Index pour chercher par créneau disponible avec statut actif
        builder.HasIndex(rs => new { rs.AvailableSlotId, rs.ReleasedAt })
            .HasDatabaseName("idx_reservation_slots_available_active");

        // Index partiel pour les créneaux actifs (ReleasedAt IS NULL)
        builder.HasIndex(rs => rs.ReleasedAt)
            .HasDatabaseName("idx_reservation_slots_active")
            .HasFilter("\"ReleasedAt\" IS NULL");
    }
}