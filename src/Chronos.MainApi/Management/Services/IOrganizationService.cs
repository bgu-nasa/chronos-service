using Chronos.Domain.Management;

namespace Chronos.MainApi.Management.Services;

public interface IOrganizationService
{
    Task<Guid> CreateOrganizationAsync(string name);
    Task<Organization> GetOrganizationAsync(Guid organizationId);
    Task UpdateOrganizationAsync(Guid organizationId, string name);
    Task SetForDeletionAsync(Guid organizationId);
    Task RestoreDeletedOrganizationAsync(Guid organizationId);
}