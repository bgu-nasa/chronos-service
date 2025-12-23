using Chronos.Domain.Schedule;

namespace Chronos.Data.Repositories.Schedule;

public interface ISchedulingPeriodRepository
{
    Task<SchedulingPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<SchedulingPeriod?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<List<SchedulingPeriod>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(SchedulingPeriod schedulingPeriod, CancellationToken cancellationToken = default);

    Task UpdateAsync(SchedulingPeriod schedulingPeriod, CancellationToken cancellationToken = default);

    Task DeleteAsync(SchedulingPeriod schedulingPeriod, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

}

