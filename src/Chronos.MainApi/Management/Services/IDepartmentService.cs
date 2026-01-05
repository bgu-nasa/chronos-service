using Chronos.Domain.Management;

namespace Chronos.MainApi.Management.Services;

public interface IDepartmentService
{
    Task<Department> CreateDepartmentAsync(Guid organizationId, string name);
    Task<Department> GetDepartmentAsync(Guid organizationId, Guid departmentId);
    Task<List<Department>> GetDepartmentsAsync(Guid organizationId);
    Task UpdateDepartmentAsync(Guid organizationId, Guid departmentId, string name);
    Task SetForDeletionAsync(Guid organizationId, Guid departmentId);
    Task RestoreDeletedDepartmentAsync(Guid organizationId, Guid departmentId);
}