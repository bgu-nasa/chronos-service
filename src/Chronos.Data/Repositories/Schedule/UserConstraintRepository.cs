using Chronos.Data.Context;
using Chronos.Domain.Schedule;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Data.Repositories.Schedule;

public class UserConstraintRepository(AppDbContext context) : IUserConstraintRepository
{
    public async Task<UserConstraint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.UserConstraints
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<UserConstraint>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.UserConstraints
            .OrderBy(c => c.Key)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserConstraint?> GetByUserPeriodAsync(Guid userId, Guid schedulingPeriodId, CancellationToken cancellationToken = default)
    {
        return await context.UserConstraints
            .FirstOrDefaultAsync(c =>
                c.UserId == userId &&
                c.SchedulingPeriodId == schedulingPeriodId, cancellationToken);
    }

    public async Task<List<UserConstraint>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.UserConstraints
            .Where(c => c.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserConstraint>> GetBySchedulingPeriodIdAsync(Guid schedulingPeriodId, CancellationToken cancellationToken = default)
    {
        return await context.UserConstraints
            .Where(c => c.SchedulingPeriodId == schedulingPeriodId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(UserConstraint constraint, CancellationToken cancellationToken = default)
    {
        await context.UserConstraints.AddAsync(constraint, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UserConstraint constraint, CancellationToken cancellationToken = default)
    {
        context.UserConstraints.Update(constraint);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(UserConstraint constraint, CancellationToken cancellationToken = default)
    {
        context.UserConstraints.Remove(constraint);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.UserConstraints
            .AnyAsync(c => c.Id == id, cancellationToken);
    }
}
