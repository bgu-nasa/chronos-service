using Chronos.Domain.Schedule;

namespace Chronos.MainApi.Schedule.Services;

public interface ISchedulingPeriodService
{
    Task<Guid> CreateSchedulingPeriodAsync(Guid organizationId, string name, DateTime fromDate, DateTime toDate);

    Task<SchedulingPeriod> GetSchedulingPeriodAsync(Guid organizationId, Guid schedulingPeriodId);

    Task<List<SchedulingPeriod>> GetSchedulingPeriodsAsync(Guid organizationId);

    Task UpdateSchedulingPeriodAsync(Guid organizationId, Guid schedulingPeriodId, string name, DateTime fromDate, DateTime toDate);

    Task DeleteSchedulingPeriodAsync(Guid organizationId, Guid schedulingPeriodId);
}
