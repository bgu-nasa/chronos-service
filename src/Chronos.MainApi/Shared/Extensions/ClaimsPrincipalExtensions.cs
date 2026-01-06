using System.Security.Claims;
using System.Text.Json;
using Chronos.Domain.Management.Roles;

namespace Chronos.MainApi.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
     private const string RolesClaimType = "roles";

    public static IReadOnlyList<SimpleRoleAssignment> GetRoles(this ClaimsPrincipal principal)
    {
        var rolesClaim = principal.FindFirst(RolesClaimType);
        if (rolesClaim == null || string.IsNullOrWhiteSpace(rolesClaim.Value))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<IReadOnlyList<SimpleRoleAssignment>>(rolesClaim.Value) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }
}