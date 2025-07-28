using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Abstractions
{
    public interface IJwtProvider
    {
        string Generate(User user);
    }
}
