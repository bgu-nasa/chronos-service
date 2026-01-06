using Chronos.Domain.Management.Roles;
using Chronos.MainApi.Shared.Extensions;
using Chronos.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Chronos.MainApi.Shared.Middleware.Rbac;

/// <summary>
/// Forces the user to have a specific role within the organization that is defined in the request context.
/// </summary>
public sealed class RequireRoleOrgHandler : AuthorizationHandler<RequireOrgRole>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RequireOrgRole requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            return Task.CompletedTask;
        }

        // Sanity check to avoid forgery
        var organizationId = httpContext.GetOrganizationId();
        if (organizationId is null)
        {
            return Task.CompletedTask;
        }

        var authorized = context.User.GetRoles()
            .Where(sr => sr.DepartmentId is null)
            .Where(sr => sr.OrganizationId.ToString().Equals(organizationId))
            .Select(sr => sr.Role)
            .Any(r => r.RoleIncludes(requirement.Role));

        if (authorized)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Forces the user to have a specific role within the department that is defined in the route.
/// The route must contain a "departmentId" parameter.
/// </summary>
public sealed class RequireRoleDeptHandler : AuthorizationHandler<RequireDeptRole>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RequireDeptRole requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            return Task.CompletedTask;
        }

        // Sanity check to avoid forgery
        var organizationId = httpContext.GetOrganizationId();
        var departmentId = httpContext.GetDepartmentId();
        if (organizationId is null || departmentId is null)
        {
            return Task.CompletedTask;
        }

        var authorized = context.User.GetRoles()
            .Where(sr => sr.OrganizationId.ToString().Equals(organizationId))
            .Where(sr => sr.DepartmentId is not null)
            .Where(sr => sr.DepartmentId.ToString()!.Equals(departmentId))
            .Select(sr => sr.Role)
            .Any(r => r.RoleIncludes(requirement.Role));

        if (authorized)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}