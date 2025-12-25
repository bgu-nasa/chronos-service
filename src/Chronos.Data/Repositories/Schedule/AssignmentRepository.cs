using Chronos.Data.Context;
using Chronos.Domain.Schedule;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Data.Repositories.Schedule;

public class AssignmentRepository(AppDbContext context) : IAssignmentRepository
{
    public async Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Assignments
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<List<Assignment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Assignments
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Assignment>> GetBySlotIdAsync(Guid slotId, CancellationToken cancellationToken = default)
    {
        return await context.Assignments
            .Where(a => a.SlotId == slotId)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<Assignment>> GetBySchedulingItemIdAsync(Guid schedulingItemId, CancellationToken cancellationToken = default)
    {
        return await context.Assignments
            .Where(a => a.ScheduledItemId == schedulingItemId)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Assignment?> GetbySlotIdAndSchedulingItemIdAsync
        (Guid slotId, Guid schedulingItemId, CancellationToken cancellationToken = default)
    {
        return await context.Assignments 
            .FirstOrDefaultAsync(a => a.SlotId == slotId && a.ScheduledItemId == schedulingItemId, cancellationToken);}

    public async Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        await context.Assignments.AddAsync(assignment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        context.Assignments.Update(assignment);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        context.Assignments.Remove(assignment);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Assignments
            .AnyAsync(a => a.Id == id, cancellationToken);
    }
}
