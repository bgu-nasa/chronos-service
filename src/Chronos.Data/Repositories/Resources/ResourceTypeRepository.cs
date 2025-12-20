using Chronos.Data.Context;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Data.Repositories.Resources;

public class ResourceTypeRepository(AppDbContext context) : IResourceTypeRepository
{
    public async Task<ResourceType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.ResourceTypes
            .FirstOrDefaultAsync(rt => rt.Id == id, cancellationToken);
    }

    public async Task<List<ResourceType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.ResourceTypes
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ResourceType resourceType, CancellationToken cancellationToken = default)
    {
        await context.ResourceTypes.AddAsync(resourceType, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ResourceType resourceType, CancellationToken cancellationToken = default)
    {
        context.ResourceTypes.Update(resourceType);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ResourceType resourceType, CancellationToken cancellationToken = default)
    {
        context.ResourceTypes.Remove(resourceType);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.ResourceTypes
            .AnyAsync(rt => rt.Id == id, cancellationToken);
    }
}