namespace Chronos.MainApi.Schedule.Contracts;

public record SchedulingPeriodResponse(
    string Id,
    string Name,
    DateTime FromDate,
    DateTime ToDate);
