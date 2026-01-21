using Chronos.Domain.Resources;
using Chronos.MainApi.Resources.Services;

namespace Chronos.MainApi.Schedule.Services;

public class ExternalResourceService(
    IResourceService resourceService
) : IExternalResourceService
{
    public async Task<Resource> GetResourceAsync(Guid organizationId, Guid resourceId)
    {
        return await resourceService.GetResourceAsync(organizationId, resourceId);
    }
}
