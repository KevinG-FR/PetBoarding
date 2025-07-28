using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        }
    }
}