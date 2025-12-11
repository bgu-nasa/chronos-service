using Chronos.Domain.Course;

namespace Chronos.Data.Repositories.Course;

public interface ICourseRepository
{
    Task<Domain.Course.Course> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<List<Domain.Course.Course>> GetAllAsync(CancellationToken token = default);
    Task<Domain.Course.Course> AddAsync(
        Domain.Course.Course course,
        CancellationToken token = default
    );
    Task<Domain.Course.Course> UpdateAsync(
        Domain.Course.Course course,
        CancellationToken token = default
    );
    Task DeleteAsync(Guid id, CancellationToken token = default);
}
