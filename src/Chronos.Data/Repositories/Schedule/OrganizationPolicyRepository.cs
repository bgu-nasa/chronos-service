using Chronos.Data.Context;
using Chronos.Domain.Schedule;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Data.Repositories.Schedule;

public class OrganizationPolicyRepository(AppDbContext context) : IOrganizationPolicyRepository
{
    public async Task<OrganizationPolicy?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.OrganizationPolicies
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<OrganizationPolicy>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.OrganizationPolicies
            .OrderBy(p => p.Key)
            .ToListAsync(cancellationToken);
    }

    public async Task<OrganizationPolicy?> GetByPeriodAsync(Guid schedulingPeriodId, CancellationToken cancellationToken = default)
    {
        return await context.OrganizationPolicies
        .FirstOrDefaultAsync(p => p.SchedulingPeriodId == schedulingPeriodId, cancellationToken);
    }

    public async Task AddAsync(OrganizationPolicy policy, CancellationToken cancellationToken = default)
    {
        await context.OrganizationPolicies.AddAsync(policy, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(OrganizationPolicy policy, CancellationToken cancellationToken = default)
    {
        context.OrganizationPolicies.Update(policy);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(OrganizationPolicy policy, CancellationToken cancellationToken = default)
    {
        context.OrganizationPolicies.Remove(policy);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.OrganizationPolicies
            .AnyAsync(p => p.Id == id, cancellationToken);
    }
}
