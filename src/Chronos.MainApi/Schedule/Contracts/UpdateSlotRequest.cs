namespace Chronos.MainApi.Schedule.Contracts;

public record UpdateSlotRequest(
    WeekDays Weekday,
    TimeSpan FromTime,
    TimeSpan ToTime);
