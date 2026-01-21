using Chronos.Domain.Resources;
using Chronos.MainApi.Resources.Services;

namespace Chronos.MainApi.Schedule.Services;

public class ExternalActivityService(
    IActivityService activityService
) : IExternalActivityService
{
    public async Task<Activity> GetActivityAsync(Guid organizationId, Guid activityId)
    {
        return await activityService.GetActivityAsync(organizationId,activityId);
    }
}