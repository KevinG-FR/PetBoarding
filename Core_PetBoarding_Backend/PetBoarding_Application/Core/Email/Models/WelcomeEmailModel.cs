namespace PetBoarding_Application.Core.Email.Models;

public sealed class WelcomeEmailModel
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string LoginUrl { get; init; } = "https://petboarding.com/login";
}