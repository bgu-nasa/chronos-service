using Chronos.Domain.Management.Roles;
using Chronos.MainApi.Management.Contracts;

namespace Chronos.MainApi.Management.Services;

public interface IRoleService
{
    Task<List<RoleAssignmentResponse>> GetAllAssignmentsAsync(Guid organizationId);
    Task<List<RoleAssignmentResponse>> GetUserAssignmentsAsync(Guid organizationId, Guid userId);
    Task<RoleAssignmentResponse> GetAssignmentAsync(Guid organizationId, Guid roleAssignmentId);
    Task<RoleAssignmentResponse> AddAssignmentAsync(Guid organizationId, Guid? departmentId, Guid userId, Role role);
    Task RemoveAssignmentAsync(Guid organizationId, Guid roleAssignmentId);
}