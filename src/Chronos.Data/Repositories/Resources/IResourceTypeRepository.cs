using Chronos.Domain.Resources;

namespace Chronos.Data.Repositories.Resources;

public interface IResourceTypeRepository
{
    Task<ResourceType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ResourceType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(ResourceType resourceType, CancellationToken cancellationToken = default);
    Task UpdateAsync(ResourceType resourceType, CancellationToken cancellationToken = default);
    Task DeleteAsync(ResourceType resourceType, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}