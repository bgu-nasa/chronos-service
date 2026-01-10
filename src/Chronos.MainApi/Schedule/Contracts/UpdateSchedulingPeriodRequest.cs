namespace Chronos.MainApi.Schedule.Contracts;

public record UpdateSchedulingPeriodRequest(
    string Name,
    DateTime FromDate,
    DateTime ToDate);
