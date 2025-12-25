using Chronos.Domain.Schedule;

namespace Chronos.Data.Repositories.Schedule;

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(Guid id);

    Task<List<Assignment>> GetAllAsync();

    Task<List<Assignment>> GetBySlotIdAsync(Guid slotId);
    Task<List<Assignment>> GetBySchedulingItemIdAsync(Guid schedulingItemId);
    Task<Assignment?> GetBySlotIdAndResourceIdAsync(Guid slotId, Guid resourceId);
    
    Task AddAsync(Assignment assignment);

    Task UpdateAsync(Assignment assignment);

    Task DeleteAsync(Assignment assignment);

    Task<bool> ExistsAsync(Guid id);
}
