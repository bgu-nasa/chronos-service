using Chronos.Domain.Schedule;

namespace Chronos.Data.Repositories.Schedule;

public interface IUserPreferenceRepository
{    
    Task<UserPreference?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<UserPreference>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<UserPreference?> GetByUserPeriodAndKeyAsyn
        (Guid userId, Guid schedulingPeriodId, string key, CancellationToken cancellationToken = default);

    Task<List<UserPreference>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<List<UserPreference>> GetBySchedulingPeriodIdAsync(Guid schedulingPeriodId, CancellationToken cancellationToken = default);

    Task AddAsync(UserPreference preference, CancellationToken cancellationToken = default);

    Task UpdateAsync(UserPreference preference, CancellationToken cancellationToken = default);

    Task DeleteAsync(UserPreference preference, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}

