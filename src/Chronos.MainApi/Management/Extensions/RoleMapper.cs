using Chronos.Domain.Management.Roles;
using Chronos.MainApi.Management.Contracts;

namespace Chronos.MainApi.Management.Extensions;

public static class RoleMapper
{
    public static RoleType ToRoleType(this Role role) =>
        role switch
        {
            Role.Administrator => RoleType.Administrator,
            Role.UserManager => RoleType.UserManager,
            Role.ResourceManager => RoleType.ResourceManager,
            Role.Operator => RoleType.Operator,
            Role.Viewer => RoleType.Viewer,
            _ => throw new ArgumentOutOfRangeException(nameof(role), $"Not expected role value: {role}"),
        };

    public static Role ToDomainRole(this RoleType roleType) =>
        roleType switch
        {
            RoleType.Administrator => Role.Administrator,
            RoleType.UserManager => Role.UserManager,
            RoleType.ResourceManager => Role.ResourceManager,
            RoleType.Operator => Role.Operator,
            RoleType.Viewer => Role.Viewer,
            _ => throw new ArgumentOutOfRangeException(nameof(roleType), $"Not expected role type value: {roleType}"),
        };

    public static RoleAssignmentResponse ToRoleAssignmentResponse(this RoleAssignment assignment) =>
        new(
            Id: assignment.Id,
            UserId: assignment.UserId,
            OrganizationId: assignment.OrganizationId,
            DepartmentId: assignment.DepartmentId,
            Role: assignment.Role.ToRoleType()
        );
}