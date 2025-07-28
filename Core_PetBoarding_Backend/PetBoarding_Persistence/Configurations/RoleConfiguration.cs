using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PetBoarding_Domain.Users;

using PetBoarding_Persistence.Constants;

namespace PetBoarding_Persistence.Configurations
{
    internal class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(TableNames.Roles);

            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Permissions)
                .WithMany()
                .UsingEntity<RolePermission>();

            builder.HasMany(x => x.Users)
                .WithMany();

            builder.HasData(Role.GetValues());
        }

    }
}
