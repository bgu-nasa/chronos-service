namespace Chronos.Domain.Management.Roles;

public class RoleAssignment
{
    // Need to enforce unique constraint on (UserId, OrganizationId, DepartmentId, Role)
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public Guid OrganizationId { get; set; }
    public Guid? DepartmentId { get; set; }

    public Role Role { get; set; }

    /// <summary>
    /// Indicates whether this role assignment is system-assigned and cannot be deleted by users.
    /// </summary>
    public bool IsSystemAssigned { get; set; } = false;
}