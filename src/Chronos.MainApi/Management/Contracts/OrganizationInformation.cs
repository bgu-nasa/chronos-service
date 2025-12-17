namespace Chronos.MainApi.Management.Contracts;

/// <summary>
/// Large object which summarizes all information about an organization that is needed in another service.
/// </summary>
public record OrganizationInformation(
    Guid Id,
    string Name,
    bool Deleted,
    DateTime DeletedTime,
    RoleAssignmentResponse[] UserRoles,
    DepartmentResponse[] Departments
);