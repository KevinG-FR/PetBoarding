namespace PetBoarding_Api.Dto.Users
{
    public record LoginRequestDto
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public bool RememberMe { get; init; } = false;
    }
}
