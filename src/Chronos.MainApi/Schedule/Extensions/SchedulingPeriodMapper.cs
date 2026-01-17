using Chronos.Domain.Schedule;
using Chronos.MainApi.Schedule.Contracts;

namespace Chronos.MainApi.Schedule.Extensions;

public static class SchedulingPeriodMapper
{
    public static SchedulingPeriodResponse ToSchedulingPeriodResponse(this SchedulingPeriod period) =>
        new(
            Id: period.Id.ToString(),
            OrganizationId: period.OrganizationId.ToString(),
            Name: period.Name,
            FromDate: period.FromDate,
            ToDate: period.ToDate
        );
}
