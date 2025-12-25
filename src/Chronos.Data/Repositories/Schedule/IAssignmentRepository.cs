using Chronos.Domain.Schedule;

namespace Chronos.Data.Repositories.Schedule;

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<Assignment>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<List<Assignment>> GetBySlotIdAsync(Guid slotId, CancellationToken cancellationToken = default);
    Task<List<Assignment>> GetBySchedulingItemIdAsync(Guid schedulingItemId, CancellationToken cancellationToken = default);
    Task<Assignment?> GetbySlotIdAndSchedulingItemIdAsync
        (Guid slotId, Guid schedulingItemId, CancellationToken cancellationToken = default);
    
    Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default);

    Task UpdateAsync(Assignment assignment, CancellationToken cancellationToken = default);

    Task DeleteAsync(Assignment assignment, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
