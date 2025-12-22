using Chronos.Data.Context;
using Chronos.Domain.Schedule;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Data.Repositories.Schedule
{
    public class SchedulingPeriodRepository(AppDbContext context) : ISchedulingPeriodRepository
    {
        public async Task<SchedulingPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.SchedulingPeriods
                .FirstOrDefaultAsync(sp => sp.Id == id, cancellationToken);
        }

        public async Task<List<SchedulingPeriod>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await context.SchedulingPeriods
                .OrderBy(sp => sp.FromDate)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(SchedulingPeriod schedulingPeriod, CancellationToken cancellationToken = default)
        {
            await context.SchedulingPeriods.AddAsync(schedulingPeriod, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(SchedulingPeriod schedulingPeriod, CancellationToken cancellationToken = default)
        {
            context.SchedulingPeriods.Update(schedulingPeriod);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(SchedulingPeriod schedulingPeriod, CancellationToken cancellationToken = default)
        {
            context.SchedulingPeriods.Remove(schedulingPeriod);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.SchedulingPeriods
                .AnyAsync(sp => sp.Id == id, cancellationToken);
        }
    }
}
