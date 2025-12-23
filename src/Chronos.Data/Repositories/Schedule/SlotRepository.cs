using Chronos.Data.Context;
using Chronos.Domain.Schedule;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Data.Repositories.Schedule;

public class SlotRepository(AppDbContext context) : ISlotRepository
{
    public async Task<Slot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Slots
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<Slot>> GetBySchedulingPeriodIdAsync(Guid schedulingPeriodId, CancellationToken cancellationToken = default)
    {
        return await context.Slots
            .Where(s => s.SchedulingPeriodId == schedulingPeriodId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Slot>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Slots
            .OrderBy(s => s.Weekday)
            .ThenBy(s => s.FromTime)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Slot slot, CancellationToken cancellationToken = default)
    {
        await context.Slots.AddAsync(slot, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Slot slot, CancellationToken cancellationToken = default)
    {
        context.Slots.Update(slot);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Slot slot, CancellationToken cancellationToken = default)
    {
        context.Slots.Remove(slot);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Slots
            .AnyAsync(s => s.Id == id, cancellationToken);
    }
}
