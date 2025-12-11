using Chronos.Data.Context;
using Chronos.Domain.Course;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Data.Repositories.Course;

public class CourseRepository(AppDbContext context) : ICourseRepository
{
    public async Task<Domain.Course.Course> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        var course = await context.Courses.FirstOrDefaultAsync(x => x.Id == id, token);

        if (course is null)
        {
            throw new KeyNotFoundException($"Course with Id {id} was not found");
        }

        return course;
    }

    public async Task<List<Domain.Course.Course>> GetAllAsync(CancellationToken token = default)
    {
        return await context.Courses.ToListAsync(token);
    }

    public async Task<Domain.Course.Course> AddAsync(Domain.Course.Course course, CancellationToken token = default)
    {
        var entry = await context.Courses.AddAsync(course, token);
        await context.SaveChangesAsync(token);
        return entry.Entity;
    }

    public async Task<Domain.Course.Course> UpdateAsync(Domain.Course.Course course, CancellationToken token = default)
    {
        var entry = context.Courses.Update(course);
        await context.SaveChangesAsync(token);
        return entry.Entity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken token = default)
    {
        var course = await context.Courses.FirstOrDefaultAsync(x => x.Id == id, token);
        if (course is not null)
        {
            context.Courses.Remove(course);
            await context.SaveChangesAsync(token);
        }
    }
}
