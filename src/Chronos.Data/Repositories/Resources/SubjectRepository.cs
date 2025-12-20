using Chronos.Data.Context;
using Chronos.Domain.Resources;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Data.Repositories.Resources;

public class SubjectRepository(AppDbContext context) : ISubjectRepository
{
    public async Task<Subject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Subjects
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<Subject>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Subjects
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Subject subject, CancellationToken cancellationToken = default)
    { 
        await context.Subjects.AddAsync(subject, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        context.Subjects.Update(subject);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        context.Subjects.Remove(subject);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Subjects
            .AnyAsync(s => s.Id == id, cancellationToken);
    }
}