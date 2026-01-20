using Chronos.Data.Context;
using Chronos.Domain.Schedule;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Data.Repositories.Schedule;

public class ExternalResourceRepository(AppDbContext context) : IExternalResourceRepository
{

    public async Task<List<ExternalResource>> GetAllAsync()
    {
        return await context.ExternalResources
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.ExternalResources
            .AnyAsync(r => r.Id == id);
    }
}