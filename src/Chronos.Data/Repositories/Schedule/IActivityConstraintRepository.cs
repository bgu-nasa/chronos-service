using Chronos.Domain.Schedule;

namespace Chronos.Data.Repositories.Schedule;

public interface IActivityConstraintRepository
{
    Task<ActivityConstraint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<ActivityConstraint>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<List<ActivityConstraint>> GetByActivityIdAsync(Guid activityId,CancellationToken cancellationToken = default);

    Task AddAsync(ActivityConstraint constraint, CancellationToken cancellationToken = default);

    Task UpdateAsync(ActivityConstraint constraint, CancellationToken cancellationToken = default);

    Task DeleteAsync(ActivityConstraint constraint, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
