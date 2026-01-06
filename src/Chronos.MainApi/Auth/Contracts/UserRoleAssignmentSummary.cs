using Chronos.MainApi.Management.Contracts;

namespace Chronos.MainApi.Auth.Contracts;

public record UserRoleAssignmentSummary(string UserEmail, RoleAssignmentResponse[] Assignments);