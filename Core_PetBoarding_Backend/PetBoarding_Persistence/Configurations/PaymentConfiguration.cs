using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PetBoarding_Domain.Payments;

using PetBoarding_Persistence.Constants;

namespace PetBoarding_Persistence.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable(TableNames.Payments);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(paymentId => paymentId.Value, value => new PaymentId(value));

        builder.Property(x => x.Amount)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.Method)
            .HasConversion(
                method => method.Id,
                id => PaymentMethod.FromId(id) ?? PaymentMethod.CreditCard)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion(
                status => status.Id,
                id => PaymentStatus.FromId(id) ?? PaymentStatus.Pending)
            .IsRequired();

        builder.Property(x => x.ExternalTransactionId)
            .HasMaxLength(255);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.ProcessedAt);

        builder.Property(x => x.FailureReason)
            .HasMaxLength(1000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        // Index pour les requêtes courantes
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.ExternalTransactionId)
            .IsUnique()
            .HasFilter("\"ExternalTransactionId\" IS NOT NULL");
        builder.HasIndex(x => new { x.Status, x.CreatedAt });
    }
}