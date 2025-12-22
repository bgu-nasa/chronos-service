using Chronos.Domain.Schedule;

namespace Chronos.Data.Repositories.Schedule;

public interface ISlotRepository
{
    Task<Slot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<Slot>> GetBySchedulingPeriodIdAsync(Guid schedulingPeriodId, CancellationToken cancellationToken = default);

    Task<List<ActivityConstraint>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Slot slot, CancellationToken cancellationToken = default);

    Task UpdateAsync(Slot slot, CancellationToken cancellationToken = default);

    Task DeleteAsync(Slot slot, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}

