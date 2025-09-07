using PetBoarding_Application.Core.Abstractions;

namespace PetBoarding_Application.Core.Email.SendWelcomeEmail;

public sealed record SendWelcomeEmailCommand(
    string Email,
    string FirstName,
    string LastName
) : ICommand;