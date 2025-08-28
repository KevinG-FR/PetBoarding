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

        // Index pour optimiser les requêtes
        builder.HasIndex(rs => rs.ReservationId)
            .HasDatabaseName("IX_ReservationSlots_ReservationId");

        builder.HasIndex(rs => rs.AvailableSlotId)
            .HasDatabaseName("IX_ReservationSlots_AvailableSlotId");

        builder.HasIndex(rs => new { rs.ReservationId, rs.AvailableSlotId })
            .IsUnique()
            .HasDatabaseName("IX_ReservationSlots_ReservationId_AvailableSlotId");

        builder.HasIndex(rs => rs.ReservedAt)
            .HasDatabaseName("IX_ReservationSlots_ReservedAt");

        // Index pour les requêtes d'audit
        builder.HasIndex(rs => new { rs.ReleasedAt, rs.ReservedAt })
            .HasDatabaseName("IX_ReservationSlots_Audit");
    }
}