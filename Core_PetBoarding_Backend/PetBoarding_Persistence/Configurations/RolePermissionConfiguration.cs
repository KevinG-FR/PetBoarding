using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PetBoarding_Domain.Users;

namespace PetBoarding_Persistence.Configurations
{
    internal class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.HasKey(x => new { x.RoleId, x.PermissionId });
            builder.HasData(
                Create(Role.Registered, PetBoarding_Domain.Accounts.Permission.ReadMember),
                Create(Role.Registered, PetBoarding_Domain.Accounts.Permission.UpdateMember),
                Create(Role.Admin, PetBoarding_Domain.Accounts.Permission.ReadMember),
                Create(Role.Admin, PetBoarding_Domain.Accounts.Permission.UpdateMember),
                Create(Role.Admin, PetBoarding_Domain.Accounts.Permission.DeleteMember)
                );
        }
        private static RolePermission Create(Role role, PetBoarding_Domain.Accounts.Permission permission)
        {
            return new RolePermission
            {
                RoleId = role.Id,
                PermissionId = (int)permission
            };
        }
    }
}
