using Chronos.Data.Context;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Data.Repositories.Resources;

public class ResourceAttributeRepository(AppDbContext context) : IResourceAttributeRepository
{
    public async Task<ResourceAttribute?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.ResourceAttributes
            .FirstOrDefaultAsync(ra => ra.Id == id, cancellationToken);
    }

    public async Task<List<ResourceAttribute>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.ResourceAttributes
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ResourceAttribute resourceAttribute, CancellationToken cancellationToken = default)
    {
        await context.ResourceAttributes.AddAsync(resourceAttribute, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ResourceAttribute resourceAttribute, CancellationToken cancellationToken = default)
    {
        context.ResourceAttributes.Update(resourceAttribute);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ResourceAttribute resourceAttribute, CancellationToken cancellationToken = default)
    {
        context.ResourceAttributes.Remove(resourceAttribute);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.ResourceAttributes
            .AnyAsync(ra => ra.Id == id, cancellationToken);
    }
}