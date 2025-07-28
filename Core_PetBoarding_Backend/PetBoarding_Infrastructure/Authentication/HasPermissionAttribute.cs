using Microsoft.AspNetCore.Authorization;

using PetBoarding_Domain.Accounts;

namespace PetBoarding_Infrastructure.Authentication
{
    public sealed class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(Permission permission)
            : base(policy: permission.ToString())
        {
        }
    }
}
