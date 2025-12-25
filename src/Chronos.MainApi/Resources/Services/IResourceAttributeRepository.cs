using Chronos.Domain.Resources;

namespace Chronos.Data.Repositories.Resources;

public interface IResourceAttributeRepository
{
    Task<ResourceAttribute?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ResourceAttribute>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(ResourceAttribute resourceAttribute, CancellationToken cancellationToken = default);
    Task UpdateAsync(ResourceAttribute resourceAttribute, CancellationToken cancellationToken = default);
    Task DeleteAsync(ResourceAttribute resourceAttribute, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}