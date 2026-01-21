using Chronos.Domain.Resources;

namespace Chronos.MainApi.Schedule.Services;

public interface IExternalResourceService
{
    Task<Resource> GetResourceAsync(Guid organizationId, Guid resourceId);
}
