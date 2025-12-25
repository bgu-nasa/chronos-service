using Chronos.Data.Context;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Data.Repositories.Resources;

public class ResourceRepository(AppDbContext context) : IResourceRepository
{
    public async Task<Resource?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Resources
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<List<Resource>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Resources
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Resource resource, CancellationToken cancellationToken = default)
    {
        await context.Resources.AddAsync(resource, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Resource resource, CancellationToken cancellationToken = default)
    {
        context.Resources.Update(resource);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Resource resource, CancellationToken cancellationToken = default)
    {
        context.Resources.Remove(resource);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Resources
            .AnyAsync(r => r.Id == id, cancellationToken);
    }
}