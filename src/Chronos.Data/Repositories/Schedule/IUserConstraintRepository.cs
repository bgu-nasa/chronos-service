using Chronos.Domain.Schedule;

namespace Chronos.Data.Repositories.Schedule;
public interface IUserConstraintRepository
{
    Task<UserConstraint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<UserConstraint>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<UserConstraint?> GetByUserPeriodAsync(Guid userId, Guid schedulingPeriodId, CancellationToken cancellationToken = default);

    Task<List<UserConstraint>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<List<UserConstraint>> GetBySchedulingPeriodIdAsync(Guid schedulingPeriodId, CancellationToken cancellationToken = default);

    Task AddAsync(UserConstraint constraint, CancellationToken cancellationToken = default);

    Task UpdateAsync(UserConstraint constraint, CancellationToken cancellationToken = default);

    Task DeleteAsync(UserConstraint constraint, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
