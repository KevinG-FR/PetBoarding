using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PetBoarding_Domain.Addresses;
using PetBoarding_Domain.Users;

using PetBoarding_Persistence.Constants;

namespace PetBoarding_Persistence.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(TableNames.Users);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasConversion(userId => userId.Value, value => new UserId(value));

            builder.Property(x => x.Firstname).HasConversion(firstname => firstname.Value, value => Firstname.Create(value).Value);

            builder.Property(x => x.Lastname).HasConversion(lastname => lastname.Value, value => Lastname.Create(value).Value);

            builder.Property(x => x.Email).HasConversion(email => email.Value, value => Email.Create(value).Value);

            builder.Property(x => x.EmailConfirmed);

            builder.Property(x => x.PhoneNumber).HasConversion(phoneNumber => phoneNumber.Value, value => PhoneNumber.Create(value).Value);

            builder.Property(x => x.PhoneNumberConfirmed);

            builder.Property(x => x.PasswordHash);

            builder.Property(x => x.Status).HasConversion<int>();

            builder.Property(x => x.ProfileType).HasConversion<int>();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            // Configuration de la relation avec Address
            builder.Property(x => x.AddressId)
                .HasConversion(
                    addressId => addressId != null ? addressId.Value : (Guid?)null,
                    value => value.HasValue ? new AddressId(value.Value) : null);

            builder.HasOne(x => x.Address)
                .WithMany()  // Pas de collection inverse
                .HasForeignKey(x => x.AddressId)
                .OnDelete(DeleteBehavior.SetNull);

            // ===== PERFORMANCE INDEXES =====
            // Index composite pour l'authentification (le plus critique)
            builder.HasIndex(x => new { x.Email, x.PasswordHash })
                .HasDatabaseName("idx_users_email_password");

            // Index unique pour les recherches par email seul (contrainte d'unicité)
            builder.HasIndex(x => x.Email)
                .IsUnique()
                .HasDatabaseName("idx_users_email");

            // Index pour les filtres par statut et type de profil
            builder.HasIndex(x => new { x.Status, x.ProfileType })
                .HasDatabaseName("idx_users_status_profiletype");

            // Index pour les timestamps d'audit
            builder.HasIndex(x => x.CreatedAt)
                .HasDatabaseName("idx_users_created_at");
        }
    }
}