using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Core.Users.GetAdminAndEmployeeUsers
{
    public record GetAdminAndEmployeeUsersQuery(
        UserId CurrentUserId,
        int? ProfileTypeFilter = null
    ) : IQuery<List<User>>;
}