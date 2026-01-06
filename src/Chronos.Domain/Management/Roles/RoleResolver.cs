namespace Chronos.Domain.Management.Roles;

public static class RoleResolver
{
    private static readonly Dictionary<Role, List<Role>> RoleMaps = new()
    {
        { Role.Administrator, [Role.Administrator] },
        { Role.UserManager, [Role.UserManager, Role.Administrator] },
        { Role.ResourceManager, [Role.ResourceManager, Role.Administrator] },
        { Role.Operator, [Role.Operator, Role.ResourceManager, Role.Administrator] },
        { Role.Viewer, [/* Should never reach here */] }
    };

    /// <summary>
    /// Whether the given role includes the required role.
    /// </summary>
    /// <param name="givenRole">The given role to check.</param>
    /// <param name="requiredRole">The required role.</param>
    /// <returns>true if the given role satisfies the required role, false otherwise.</returns>
    public static bool RoleIncludes(this Role givenRole, Role requiredRole)
    {
        if (requiredRole == Role.Viewer)
        {
            return true;
        }

        return RoleMaps.TryGetValue(givenRole, out var includedRoles) && includedRoles.Contains(requiredRole);
    }
}