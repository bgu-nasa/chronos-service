using Chronos.Domain.Resources;

namespace Chronos.Data.Repositories.Resources;

public interface IResourceRepository
{
    Task<Resource?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Resource>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Resource resource, CancellationToken cancellationToken = default);
    Task UpdateAsync(Resource resource, CancellationToken cancellationToken = default);
    Task DeleteAsync(Resource resource, CancellationToken cancellationToken = default);
}