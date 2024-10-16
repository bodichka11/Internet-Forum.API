using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApp.BusinessLogic.Services.Interfaces;

namespace WebApp.BusinessLogic.Services;
public class TokenService : ITokenService
{
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var secretKeyEnv = Environment.GetEnvironmentVariable("SecretKey");

        if (string.IsNullOrEmpty(secretKeyEnv))
        {
            throw new InvalidOperationException("Secret key environment variable is not set.");
        }

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyEnv));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokeOptions = new JwtSecurityToken(
            issuer: "https://localhost:7070",
            audience: "https://localhost:7070",
            claims: claims,
            expires: DateTime.Now.AddMinutes(20),
            signingCredentials: signinCredentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return tokenString;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var secretKeyEnv = Environment.GetEnvironmentVariable("SecretKey");

        if (string.IsNullOrEmpty(secretKeyEnv))
        {
            throw new InvalidOperationException("Secret key environment variable is not set.");
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKeyEnv)),
            ValidateLifetime = true,
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}
