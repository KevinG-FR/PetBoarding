using PetBoarding_Application.Core.Abstractions;
using PetBoarding_Domain.Users;

namespace PetBoarding_Application.Core.Users.GetUserByEmail;

public record GetUserByEmailQuery(string Email) : IQuery<User> { }