namespace PetBoarding_Infrastructure.Options
{
    public class JwtOptions
    {
        public string? Key { get; init; }
        public string? Issuer { get; init; }
        public string? Audience { get; init; }
        
        /// <summary>
        /// Durée de vie du token d'accès en minutes
        /// </summary>
        public int AccessTokenExpiryMinutes { get; init; } = 5; // 5 minutes par défaut

        /// <summary>
        /// Durée de vie du refresh token en minutes
        /// </summary>
        public int RefreshTokenExpiryMinutes { get; init; } = 43200; // 30 jours par défaut
    }
}
