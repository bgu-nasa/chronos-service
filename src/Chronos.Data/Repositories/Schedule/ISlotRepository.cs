using Chronos.Domain.Schedule;

namespace Chronos.Data.Repositories.Schedule;

public interface ISlotRepository
{
    Task<Slot?> GetByIdAsync(Guid id);

    Task<List<Slot>> GetBySchedulingPeriodIdAsync(Guid schedulingPeriodId);

    Task<List<Slot>> GetAllAsync();

    Task AddAsync(Slot slot);

    Task UpdateAsync(Slot slot);

    Task DeleteAsync(Slot slot);

    Task<bool> ExistsAsync(Guid id);
}

