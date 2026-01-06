namespace Chronos.MainApi.Management.Contracts;

public record RoleAssignmentResponse(Guid Id, Guid UserId, Guid OrganizationId, Guid? DepartmentId, RoleType Role);