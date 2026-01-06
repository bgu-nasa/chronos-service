using System.Security.Claims;
using System.Text.Json;
using Chronos.Domain.Management.Roles;
using Chronos.Shared.Exceptions;

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

    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            throw new TokenMissingValueException("UserId");
        }

        if (!Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new TokenMissingValueException("UserId (invalid format)");
        }

        return userId;
    }

    public static Guid GetOrganizationId(this ClaimsPrincipal principal)
    {
        var organizationClaim = principal.FindFirst("organization");
        if (organizationClaim == null)
        {
            throw new TokenMissingValueException("OrganizationId");
        }

        if (!Guid.TryParse(organizationClaim.Value, out var organizationId))
        {
            throw new TokenMissingValueException("OrganizationId (invalid format)");
        }

        return organizationId;
    }
}