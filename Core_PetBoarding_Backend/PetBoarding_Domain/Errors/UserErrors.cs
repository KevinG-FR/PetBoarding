using FluentResults;

using PetBoarding_Domain.Users;

namespace PetBoarding_Domain.Errors
{
    public static class UserErrors
    {
        public static Error NotFound() =>
            new($"User not found.");

        public static Error NotFound(Guid userId) =>
            new($"User not found for id '{userId}'.");

        public static Error ChangeUserStatus(UserStatus actualUserStatus, UserStatus newUserStatus) =>
            new($"Unable to change user status '{actualUserStatus}' for '{newUserStatus}'.");

        public static Error EmailAlreadyUsed(string email) =>
            new($"this email '{email}' is already used.");

        public static Error AuthentificationFailed() =>
            new($"No user account with this login");
    }
}
