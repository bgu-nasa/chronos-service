namespace Chronos.MainApi.Management.Contracts;

public record RoleAssignmentResponse(Guid OrganizationId, Guid? DepartmentId, RoleType Role);