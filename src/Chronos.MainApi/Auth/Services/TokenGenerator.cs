using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Chronos.Domain.Auth;
using Chronos.MainApi.Auth.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Chronos.MainApi.Auth.Services;

/// <summary>
/// Generates jwt-token.
/// </summary>
public class TokenGenerator(IOptions<AuthConfiguration> config) : ITokenGenerator
{
    // Stateless singleton
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly AuthConfiguration _config = config.Value;
    
    public string GenerateToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_config.SecretKey);
        var cred = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Issuer = _config.Issuer,
            Audience = _config.Audience,
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("organization", user.OrganizationId.ToString()),
            }),
            SigningCredentials = cred,
            Expires = DateTime.Now.AddMinutes(_config.ExpiryMinutes)
        };
        
        var token = _tokenHandler.CreateToken(tokenDescriptor);

        return _tokenHandler.WriteToken(token);
    }
}