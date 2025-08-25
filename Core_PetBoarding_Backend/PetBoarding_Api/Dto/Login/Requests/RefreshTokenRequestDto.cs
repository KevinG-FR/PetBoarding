namespace PetBoarding_Api.Dto.Login.Requests
{
    public record RefreshTokenRequestDto
    {
        public string RefreshToken { get; init; } = string.Empty;       
    }
}

