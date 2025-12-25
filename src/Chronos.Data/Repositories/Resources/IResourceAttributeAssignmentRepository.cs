using Chronos.Domain.Resources;

namespace Chronos.Data.Repositories.Resources;

public interface IResourceAttributeAssignmentRepository
{
    Task<ResourceAttributeAssignment?> GetByIdAsync(Guid resourceId, Guid resourceAttributeId, CancellationToken cancellationToken = default);
    Task<List<ResourceAttributeAssignment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(ResourceAttributeAssignment resourceAttributeAssignment, CancellationToken cancellationToken = default);
    Task UpdateAsync(ResourceAttributeAssignment resourceAttributeAssignment, CancellationToken cancellationToken = default);
    Task DeleteAsync(ResourceAttributeAssignment resourceAttributeAssignment, CancellationToken cancellationToken = default);
}