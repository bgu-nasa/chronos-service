using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Chronos.Domain.Auth;
using Chronos.Domain.Management.Roles;
using Chronos.MainApi.Auth.Configuration;
using Chronos.MainApi.Management.Extensions;
using Chronos.MainApi.Management.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Chronos.MainApi.Auth.Services;

/// <summary>
/// Generates jwt-token.
/// </summary>
public class TokenGenerator(IOptions<AuthConfiguration> config, IRoleService roleService) : ITokenGenerator
{
    // Stateless singleton (but it's not?)
    private readonly JwtSecurityTokenHandler _tokenHandler = new(); // TODO DI?
    private readonly AuthConfiguration _config = config.Value;

    private async Task<List<SimpleRoleAssignment>> GetUserRolesAsync(User user)
    {
        var roles = await roleService.GetUserAssignmentsAsync(user.OrganizationId, user.Id);
        return roles
            .Select(r => new SimpleRoleAssignment(r.Role.ToDomainRole(), r.OrganizationId, r.DepartmentId))
            .ToList();
    }

    private async Task<string> GetUserRolesSerializedAsync(User user)
    {
        var roles = await GetUserRolesAsync(user);
        return JsonSerializer.Serialize(roles);
    }

    public async Task<string> GenerateTokenAsync(User user)
    {
        var key = Encoding.UTF8.GetBytes(_config.SecretKey);
        var cred = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Issuer = _config.Issuer,
            Audience = _config.Audience,
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("organization", user.OrganizationId.ToString()),
                new Claim("roles", await GetUserRolesSerializedAsync(user))
            ]),
            SigningCredentials = cred,
            Expires = DateTime.Now.AddMinutes(_config.ExpiryMinutes)
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);

        return _tokenHandler.WriteToken(token);
    }
}