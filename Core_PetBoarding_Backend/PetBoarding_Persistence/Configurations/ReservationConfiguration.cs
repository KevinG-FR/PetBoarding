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
            .HasColumnType("datetime2");

        builder.Property(r => r.UpdatedAt)
            .HasColumnType("datetime2");

        builder.Property(r => r.TotalPrice)
            .HasColumnType("decimal(10,2)");

        builder.Property(r => r.PaidAt)
            .HasColumnType("datetime2");

        // Index for query optimization
        builder.HasIndex(r => r.UserId)
            .HasDatabaseName("IX_Reservations_UserId");

        builder.HasIndex(r => r.ServiceId)
            .HasDatabaseName("IX_Reservations_ServiceId");

        builder.HasIndex(r => r.StartDate)
            .HasDatabaseName("IX_Reservations_StartDate");

        builder.HasIndex(r => r.Status)
            .HasDatabaseName("IX_Reservations_Status");

        builder.HasIndex(r => new { r.StartDate, r.EndDate })
            .HasDatabaseName("IX_Reservations_DateRange");
    }
}
