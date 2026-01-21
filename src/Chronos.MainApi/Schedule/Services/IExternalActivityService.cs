using Chronos.Domain.Resources;
namespace Chronos.MainApi.Schedule.Services;

public interface IExternalActivityService
{
    Task<Activity> GetActivityAsync(Guid organizationId, Guid activityId);
}
