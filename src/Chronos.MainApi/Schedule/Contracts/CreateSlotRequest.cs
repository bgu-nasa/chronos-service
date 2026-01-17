namespace Chronos.MainApi.Schedule.Contracts;

public record CreateSlotRequest(
    Guid SchedulingPeriodId,
    WeekDays Weekday,
    TimeSpan FromTime,
    TimeSpan ToTime);
