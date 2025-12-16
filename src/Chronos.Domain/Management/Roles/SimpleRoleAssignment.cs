namespace Chronos.Domain.Management.Roles;

public record SimpleRoleAssignment(Role Role, Guid OrganizationId, Guid? DepartmentId);