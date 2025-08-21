using PetBoarding_Api.Dto.Addresses;

namespace PetBoarding_Api.Dto.Login.Responses
{
    public record LoginResponseDto
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public string? Token { get; init; }
        public UserDto? User { get; init; }
    }    
}
