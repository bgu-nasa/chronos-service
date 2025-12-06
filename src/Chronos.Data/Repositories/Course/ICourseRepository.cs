namespace Chronos.Data.Repositories.Course;

public interface ICourseRepository
{
    Task<Course> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<List<Course>> GetAllAsync(CancellationToken token = default);
    Task<Course> AddAsync(Course course, CancellationToken token = default);
    Task<Course> UpdateAsync(Course course, CancellationToken token = default);
    Task DeleteAsync(Guid id, CancellationToken token = default);
}
