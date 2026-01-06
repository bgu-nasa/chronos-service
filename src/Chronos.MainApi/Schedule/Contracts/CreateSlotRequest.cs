namespace Chronos.MainApi.Schedule.Contracts;

public record CreateSlotRequest(
    Guid SchedulingPeriodId,
    string Weekday,
    TimeSpan FromTime,
    TimeSpan ToTime);
