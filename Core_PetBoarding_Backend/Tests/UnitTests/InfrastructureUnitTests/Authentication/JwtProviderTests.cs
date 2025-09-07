using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetBoarding_Domain.Users;
using PetBoarding_Infrastructure.Authentication;
using PetBoarding_Infrastructure.Options;

namespace InfrastructureTests.Authentication;

public class JwtProviderTests
{
    private readonly JwtProvider _jwtProvider;
    private readonly JwtOptions _jwtOptions;
    private readonly User _testUser;

    public JwtProviderTests()
    {
        _jwtOptions = new JwtOptions
        {
            Key = "ThisIsATestSecretKeyThatShouldBeAtLeast32CharactersLong",
            Issuer = "test-issuer",
            Audience = "test-audience",
            AccessTokenExpiryMinutes = 60,
            RefreshTokenExpiryMinutes = 43200
        };

        var optionsMock = new Mock<IOptions<JwtOptions>>();
        optionsMock.Setup(x => x.Value).Returns(_jwtOptions);
        
        _jwtProvider = new JwtProvider(optionsMock.Object);
        
        _testUser = new User(
            Firstname.Create("John").Value,
            Lastname.Create("Doe").Value,
            PetBoarding_Domain.Users.Email.Create("test@example.com").Value,
            PhoneNumber.Create("1234567890").Value,
            "password-hash",
            UserProfileType.Customer
        );
    }

    [Fact]
    public void Generate_WithValidUser_ShouldReturnValidToken()
    {
        // Act
        var token = _jwtProvider.Generate(_testUser);
        
        // Assert
        token.Should().NotBeNullOrEmpty();
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        jwtToken.Issuer.Should().Be(_jwtOptions.Issuer);
        jwtToken.Audiences.Should().Contain(_jwtOptions.Audience);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == _testUser.Id.Value.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == _testUser.Email.Value);
    }

    [Fact]
    public void Generate_WithCustomDuration_ShouldUseProvidedDuration()
    {
        // Arrange
        var customDurationMinutes = 30;
        var beforeGeneration = DateTime.UtcNow;
        
        // Act
        var token = _jwtProvider.Generate(_testUser, customDurationMinutes);
        
        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var expectedExpiry = beforeGeneration.AddMinutes(customDurationMinutes);
        jwtToken.ValidTo.Should().BeCloseTo(expectedExpiry, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void Generate_WithoutCustomDuration_ShouldUseDefaultDuration()
    {
        // Arrange
        var beforeGeneration = DateTime.UtcNow;
        
        // Act
        var token = _jwtProvider.Generate(_testUser);
        
        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        var expectedExpiry = beforeGeneration.AddMinutes(_jwtOptions.AccessTokenExpiryMinutes);
        jwtToken.ValidTo.Should().BeCloseTo(expectedExpiry, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public void Generate_WithNullSecretKey_ShouldThrowException()
    {
        // Arrange
        var optionsWithNullKey = new JwtOptions
        {
            Key = null,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience
        };
        
        var optionsMock = new Mock<IOptions<JwtOptions>>();
        optionsMock.Setup(x => x.Value).Returns(optionsWithNullKey);
        
        var jwtProviderWithNullKey = new JwtProvider(optionsMock.Object);
        
        // Act & Assert
        var act = () => jwtProviderWithNullKey.Generate(_testUser);
        act.Should().Throw<Exception>().WithMessage("Secret key for JWT is null");
    }

    [Fact]
    public async Task ValidateRefreshToken_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var refreshToken = _jwtProvider.Generate(_testUser, _jwtOptions.RefreshTokenExpiryMinutes);
        
        // Act
        var result = await _jwtProvider.ValidateRefreshToken(refreshToken);
        
        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateRefreshToken_WithExpiredToken_ShouldReturnFalse()
    {
        // Arrange
        var expiredToken = GenerateExpiredToken();
        
        // Act
        var result = await _jwtProvider.ValidateRefreshToken(expiredToken);
        
        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateRefreshToken_WithMalformedToken_ShouldReturnFalse()
    {
        // Arrange
        var malformedToken = "invalid.token.format";
        
        // Act
        var result = await _jwtProvider.ValidateRefreshToken(malformedToken);
        
        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateRefreshToken_WithWrongSignature_ShouldReturnFalse()
    {
        // Arrange
        var tokenWithWrongSignature = GenerateTokenWithWrongSignature();
        
        // Act
        var result = await _jwtProvider.ValidateRefreshToken(tokenWithWrongSignature);
        
        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetClaimsFromToken_WithValidToken_ShouldReturnClaims()
    {
        // Arrange
        var token = _jwtProvider.Generate(_testUser);
        
        // Act
        var claims = _jwtProvider.GetClaimsFromToken(token);
        
        // Assert
        claims.Should().NotBeEmpty();
        claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == _testUser.Id.Value.ToString());
        claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == _testUser.Email.Value);
    }

    [Fact]
    public void GetClaimsFromToken_WithMalformedToken_ShouldThrowException()
    {
        // Arrange
        var malformedToken = "invalid.token.format";
        
        // Act & Assert
        var act = () => _jwtProvider.GetClaimsFromToken(malformedToken);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GetRefreshTokenExpiryMinutes_ShouldReturnConfiguredValue()
    {
        // Act
        var result = _jwtProvider.GetRefreshTokenExpiryMinutes();
        
        // Assert
        result.Should().Be(_jwtOptions.RefreshTokenExpiryMinutes);
    }

    [Fact]
    public async Task GenerateNewToken_WithValidRefreshToken_ShouldReturnNewToken()
    {
        // Arrange
        // Créer un refresh token valide (avec les bonnes propriétés pour passer la validation)
        var refreshToken = GenerateValidRefreshToken();
        
        // Act
        var newToken = await _jwtProvider.GenerateNewToken(refreshToken);
        
        // Assert
        newToken.Should().NotBeNullOrEmpty();
        newToken.Should().NotBe(refreshToken);
        
        // Vérifier que le nouveau token peut être lu et analysé
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(newToken);
        jwtToken.Should().NotBeNull();
        
        // Vérifier que le token a une expiration appropriée selon la configuration
        jwtToken.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpiryMinutes), TimeSpan.FromMinutes(2));
        
        // Vérifier l'issuer et audience
        jwtToken.Issuer.Should().Be(_jwtOptions.Issuer);
        jwtToken.Audiences.Should().Contain(_jwtOptions.Audience);
    }

    [Fact]
    public async Task GenerateNewToken_WithInvalidRefreshToken_ShouldThrowSecurityTokenException()
    {
        // Arrange
        var invalidToken = "invalid.token.format";
        
        // Act & Assert
        var act = async () => await _jwtProvider.GenerateNewToken(invalidToken);
        await act.Should().ThrowAsync<SecurityTokenException>().WithMessage("Invalid refresh token");
    }

    private string GenerateExpiredToken()
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, _testUser.Id.Value.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, _testUser.Email.Value)
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key!)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(-1), // Expired 1 minute ago
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateTokenWithWrongSignature()
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, _testUser.Id.Value.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, _testUser.Email.Value)
        };

        // Use a different key for wrong signature
        var wrongKey = "WrongSecretKeyForTestingPurposes32Chars";
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(wrongKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateValidRefreshToken()
    {
        // Créer un refresh token valide avec les mêmes paramètres que l'implémentation attend
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, _testUser.Id.Value.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, _testUser.Email.Value)
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key!)),
            SecurityAlgorithms.HmacSha256);

        // Créer un token avec une durée longue pour qu'il soit valide pendant le test
        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshTokenExpiryMinutes),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}