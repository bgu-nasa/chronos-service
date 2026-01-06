namespace Chronos.MainApi.Schedule.Contracts;

public record UpdateSlotRequest(
    string Weekday,
    TimeSpan FromTime,
    TimeSpan ToTime);
