using Chronos.Data.Context;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Data.Repositories.Resources;

public class ResourceAttributeAssignmentRepository(AppDbContext context) : IResourceAttributeAssignmentRepository
{
    public async Task<ResourceAttributeAssignment?> GetByIdAsync(Guid resourceId, Guid resourceAttributeId, CancellationToken cancellationToken = default)
    {
        return await context.ResourceAttributeAssignments
            .FirstOrDefaultAsync(raa => raa.ResourceId == resourceId && raa.ResourceAttributeId == resourceAttributeId, cancellationToken);
    }

    public async Task<List<ResourceAttributeAssignment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.ResourceAttributeAssignments
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ResourceAttributeAssignment resourceAttributeAssignment, CancellationToken cancellationToken = default)
    {
        await context.ResourceAttributeAssignments.AddAsync(resourceAttributeAssignment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ResourceAttributeAssignment resourceAttributeAssignment,
        CancellationToken cancellationToken = default)
    {
        context.ResourceAttributeAssignments.Update(resourceAttributeAssignment);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ResourceAttributeAssignment resourceAttributeAssignment,
        CancellationToken cancellationToken = default)
    {
        context.ResourceAttributeAssignments.Remove(resourceAttributeAssignment);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid resourceId, Guid resourceAttributeId, CancellationToken cancellationToken = default)
    {
        return await context.ResourceAttributeAssignments
            .AnyAsync(raa => raa.ResourceId == resourceId && raa.ResourceAttributeId == resourceAttributeId, cancellationToken);
    }
}