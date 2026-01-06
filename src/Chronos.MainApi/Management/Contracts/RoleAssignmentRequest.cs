namespace Chronos.MainApi.Management.Contracts;

public record RoleAssignmentRequest(Guid? DepartmentId, Guid UserId, RoleType Role);
