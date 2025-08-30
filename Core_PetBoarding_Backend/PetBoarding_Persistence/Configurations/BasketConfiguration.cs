using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PetBoarding_Domain.Baskets;
using PetBoarding_Domain.Payments;
using PetBoarding_Domain.Users;

using PetBoarding_Persistence.Constants;

namespace PetBoarding_Persistence.Configurations;

internal sealed class BasketConfiguration : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        builder.ToTable(TableNames.Baskets);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(basketId => basketId.Value, value => new BasketId(value));

        builder.Property(x => x.UserId)
            .HasConversion(userId => userId.Value, value => new UserId(value))
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion(
                status => status.Id,
                id => BasketStatus.FromId(id) ?? BasketStatus.Created)
            .IsRequired();

        builder.Property(x => x.PaymentId)
            .HasConversion(
                paymentId => paymentId != null ? paymentId.Value : (Guid?)null,
                value => value.HasValue ? new PaymentId(value.Value) : null);

        builder.Property(x => x.PaymentFailureCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        // Relations
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Payment)
            .WithMany()
            .HasForeignKey(x => x.PaymentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(i => i.BasketId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index pour les requêtes courantes
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.PaymentId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.Status, x.CreatedAt });
    }
}