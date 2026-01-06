using Chronos.Domain.Resources;

namespace Chronos.MainApi.Resources.Services;

public interface IResourceAttributeService
{
    Task<ResourceAttribute> CreateResourceAttributeAsync(Guid organizationId, string title, string description);
    Task<ResourceAttribute> GetResourceAttributeAsync(Guid organizationId, Guid resourceAttributeId);
    Task<List<ResourceAttribute>> GetResourceAttributesAsync(Guid organizationId);
    Task UpdateResourceAttributeAsync(Guid organizationId, Guid resourceAttributeId, string title, string description);
    Task DeleteResourceAttributeAsync(Guid organizationId, Guid resourceAttributeId);
}