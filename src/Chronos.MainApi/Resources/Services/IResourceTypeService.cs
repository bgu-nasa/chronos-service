using Chronos.Domain.Resources;

namespace Chronos.MainApi.Resources.Services;

public interface IResourceTypeService
{
    Task<ResourceType> CreateResourceTypeAsync(Guid organizationId, string type);
    Task<ResourceType> GetResourceTypeAsync(Guid organizationId, Guid resourceTypeId);
    Task<List<ResourceType>> GetResourceTypesAsync(Guid organizationId);
    Task UpdateResourceTypeAsync(Guid organizationId, Guid resourceTypeId, string type);
    Task DeleteResourceTypeAsync(Guid organizationId, Guid resourceTypeId);
}