using Chronos.Data.Context;
using Chronos.Domain.Schedule;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Data.Repositories.Schedule;

public class ActivityConstraintRepository(AppDbContext context) : IActivityConstraintRepository
{
    public async Task<ActivityConstraint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.ActivityConstraints
        .FirstOrDefaultAsync(ac => ac.Id == id, cancellationToken);
    }

    public async Task<List<ActivityConstraint>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.ActivityConstraints.ToListAsync(cancellationToken);
    }

    public async Task<List<ActivityConstraint>> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default)
    {
        return await context.ActivityConstraints
            .Where(ac => ac.ActivityId == activityId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ActivityConstraint constraint, CancellationToken cancellationToken = default)
    {
        await context.ActivityConstraints.AddAsync(constraint, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ActivityConstraint constraint, CancellationToken cancellationToken = default)
    {
        context.ActivityConstraints.Update(constraint);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ActivityConstraint constraint, CancellationToken cancellationToken = default)
    {
        context.ActivityConstraints.Remove(constraint);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.ActivityConstraints.AnyAsync(ac => ac.Id == id, cancellationToken);
    }
}
