using Chronos.Domain.Management.Roles;

namespace Chronos.MainApi.Management.Services;

public interface IRoleService
{
    Task<List<RoleAssignment>> GetAllAssignments(Guid organizationId);
    Task<List<RoleAssignment>> GetUserAssignments(Guid organizationId, Guid userId);
    Task<RoleAssignment> GetAssignment(Guid organizationId, Guid roleAssignmentId);
    Task<RoleAssignment> AddAssignment(Guid organizationId, Guid? departmentId, Guid userId, Role role);
    Task RemoveAssignment(Guid organizationId, Guid roleAssignmentId);
}