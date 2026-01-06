namespace Chronos.MainApi.Management.Contracts;

/// <summary>
/// Large object which summarizes all information about an organization that is needed in another service.
/// </summary>
public record OrganizationInformation(
    // Organization info
    Guid Id,
    string Name,
    bool Deleted,
    DateTime DeletedTime,

    // Current user info
    string UserEmail,
    string UserFullName,
    string? AvatarUrl,

    // Sub-objects info
    RoleAssignmentResponse[] UserRoles,
    DepartmentResponse[] Departments
);