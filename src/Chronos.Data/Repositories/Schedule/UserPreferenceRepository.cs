using Chronos.Data.Context;
using Chronos.Domain.Schedule;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Data.Repositories.Schedule;

public class UserPreferenceRepository(AppDbContext context) : IUserPreferenceRepository
{
    public async Task<UserPreference?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.UserPreferences
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<UserPreference>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.UserPreferences
            .OrderBy(p => p.Key)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserPreference?> GetByUserPeriodAsync(Guid userId, Guid schedulingPeriodId, CancellationToken cancellationToken = default)
    {
        return await context.UserPreferences
            .FirstOrDefaultAsync(p =>
                p.UserId == userId &&
                p.SchedulingPeriodId == schedulingPeriodId, cancellationToken);
    }

    public async Task<List<UserPreference>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.UserPreferences
            .Where(p => p.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserPreference>> GetBySchedulingPeriodIdAsync(Guid schedulingPeriodId, CancellationToken cancellationToken = default)
    {
        return await context.UserPreferences
            .Where(p => p.SchedulingPeriodId == schedulingPeriodId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(UserPreference preference, CancellationToken cancellationToken = default)
    {
        await context.UserPreferences.AddAsync(preference, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UserPreference preference, CancellationToken cancellationToken = default)
    {
        context.UserPreferences.Update(preference);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(UserPreference preference, CancellationToken cancellationToken = default)
    {
        context.UserPreferences.Remove(preference);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.UserPreferences
            .AnyAsync(p => p.Id == id, cancellationToken);
    }
}
