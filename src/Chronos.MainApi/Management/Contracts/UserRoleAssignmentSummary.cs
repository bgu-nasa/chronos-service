namespace Chronos.MainApi.Management.Contracts;

public record UserRoleAssignmentSummary(string UserEmail, RoleAssignmentResponse[] Assignments);