namespace PetBoarding_Api.Dto.Login.Responses
{
    public record RefreshTokenResponseDto
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public string? Token { get; init; }
    }    
}
