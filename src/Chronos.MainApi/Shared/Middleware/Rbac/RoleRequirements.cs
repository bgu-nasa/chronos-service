using Chronos.Domain.Management.Roles;
using Microsoft.AspNetCore.Authorization;

namespace Chronos.MainApi.Shared.Middleware.Rbac;

public class RequireOrgRole(Role role) : IAuthorizationRequirement
{
    public Role Role { get; } = role;
}

public class RequireDeptRole(Role role) : IAuthorizationRequirement
{
    public Role Role { get; } = role;
}
