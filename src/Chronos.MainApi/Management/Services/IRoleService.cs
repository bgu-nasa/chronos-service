using Chronos.Domain.Management.Roles;

namespace Chronos.MainApi.Management.Services;

public interface IRoleService
{
    Task<List<RoleAssignment>> GetAllAssignmentsAsync(Guid organizationId);
    Task<List<RoleAssignment>> GetUserAssignmentsAsync(Guid organizationId, Guid userId);
    Task<RoleAssignment> GetAssignmentAsync(Guid organizationId, Guid roleAssignmentId);
    Task<RoleAssignment> AddAssignmentAsync(Guid organizationId, Guid? departmentId, Guid userId, Role role);
    Task RemoveAssignmentAsync(Guid organizationId, Guid roleAssignmentId);
}