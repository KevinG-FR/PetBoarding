namespace PetBoarding_Api.Dto.Users
{
    public record LoginResponseDto
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public string? Token { get; init; }
        public UserDto? User { get; init; }
    }

    public record UserDto
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public bool EmailConfirmed { get; init; }
        public bool PhoneNumberConfirmed { get; init; }
        public string ProfileType { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}
