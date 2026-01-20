using Chronos.Domain.Schedule;

namespace Chronos.Data.Repositories.Schedule;

public interface IExternalResourceRepository
{
    Task<List<ExternalResource>> GetAllAsync();
    Task<bool> ExistsAsync(Guid id);

}