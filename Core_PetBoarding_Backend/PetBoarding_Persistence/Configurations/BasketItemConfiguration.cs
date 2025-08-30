using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Reservations;

using PetBoarding_Persistence.Constants;

namespace PetBoarding_Persistence.Configurations;

internal sealed class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
{
    public void Configure(EntityTypeBuilder<BasketItem> builder)
    {
        builder.ToTable(TableNames.BasketItems);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(basketItemId => basketItemId.Value, value => new BasketItemId(value));

        builder.Property(x => x.BasketId)
            .HasConversion(basketId => basketId.Value, value => new BasketId(value))
            .IsRequired();

        builder.Property(x => x.ReservationId)
            .HasConversion(reservationId => reservationId.Value, value => new ReservationId(value))
            .IsRequired();


        builder.Property(x => x.AddedAt)
            .IsRequired();

        // Relations
        builder.HasOne(x => x.Reservation)
            .WithMany()
            .HasForeignKey(x => x.ReservationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index pour les requêtes courantes
        builder.HasIndex(x => x.BasketId);
        builder.HasIndex(x => x.ReservationId);
        builder.HasIndex(x => new { x.BasketId, x.ReservationId })
            .IsUnique(); // Un panier ne peut avoir qu'un seul item pour une réservation donnée
    }
}