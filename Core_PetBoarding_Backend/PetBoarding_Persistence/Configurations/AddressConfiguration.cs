using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PetBoarding_Domain.Addresses;
using PetBoarding_Persistence.Constants;

namespace PetBoarding_Persistence.Configurations
{
    internal class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable(TableNames.Addresses);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasConversion(addressId => addressId.Value, value => new AddressId(value));

            builder.Property(x => x.StreetNumber)
                .HasConversion(streetNumber => streetNumber.Value, value => StreetNumber.Create(value).Value)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.StreetName)
                .HasConversion(streetName => streetName.Value, value => StreetName.Create(value).Value)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.City)
                .HasConversion(city => city.Value, value => City.Create(value).Value)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.PostalCode)
                .HasConversion(postalCode => postalCode.Value, value => PostalCode.Create(value).Value)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Country)
                .HasConversion(country => country.Value, value => Country.Create(value).Value)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Complement)
                .HasConversion(
                    complement => complement != null ? complement.Value : null,
                    value => value != null ? Complement.Create(value).Value : null)
                .HasMaxLength(200);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();
        }
    }
}
