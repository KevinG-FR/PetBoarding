using PetBoarding_Application.Abstractions;

namespace PetBoarding_Application.Email.SendWelcomeEmail;

public sealed record SendWelcomeEmailCommand(
    string Email,
    string FirstName,
    string LastName
) : ICommand;