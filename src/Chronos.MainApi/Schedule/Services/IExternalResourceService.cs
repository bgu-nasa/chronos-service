using Chronos.Domain.Schedule;

namespace Chronos.MainApi.Schedule.Services;

public interface IExternalResourceService
{
    Task<List<ExternalResource>> GetResourcesAsync(Guid organizationId);
    Task<bool> ExternalResourceExistsAsync(Guid id , Guid organizationId);
}