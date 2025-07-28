using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PetBoarding_Domain.Users;

using PetBoarding_Persistence.Constants;

namespace PetBoarding_Persistence.Configurations
{
    internal class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable(TableNames.Permissions);

            builder.HasKey(x => x.Id);

            IEnumerable<Permission> permissions = Enum.GetValues<PetBoarding_Domain.Accounts.Permission>()
                .Select(x => new Permission
                {
                    Id = (int)x,
                    Name = x.ToString()
                });

            builder.HasData(permissions);
        }
    }
}
