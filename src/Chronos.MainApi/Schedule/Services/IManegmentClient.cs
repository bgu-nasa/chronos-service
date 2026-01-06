using Chronos.Domain.Schedule;

namespace Chronos.MainApi.Schedule.Services;

public interface IManegmentClient
{
    Task ValidateOrganizationAsync(Guid organizationId);
}