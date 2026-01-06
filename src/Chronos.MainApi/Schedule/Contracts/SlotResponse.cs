namespace Chronos.MainApi.Schedule.Contracts;

public record SlotResponse(
    string Id,
    string OrganizationId,
    string SchedulingPeriodId,
    string Weekday,
    TimeSpan FromTime,
    TimeSpan ToTime);
