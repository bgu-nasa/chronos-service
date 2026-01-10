using Chronos.Domain.Schedule;
using Chronos.MainApi.Schedule.Contracts;

namespace Chronos.MainApi.Schedule.Extensions;

public static class UserPreferenceMapper
{
    public static UserPreferenceResponse ToUserPreferenceResponse(this UserPreference preference) =>
        new(
            Id: preference.Id.ToString(),
            OrganizationId: preference.OrganizationId.ToString(),
            UserId: preference.UserId.ToString(),
            SchedulingPeriodId: preference.SchedulingPeriodId.ToString(),
            Key: preference.Key,
            Value: preference.Value
        );
}
