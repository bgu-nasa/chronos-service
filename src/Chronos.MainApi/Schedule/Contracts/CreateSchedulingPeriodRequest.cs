namespace Chronos.MainApi.Schedule.Contracts;

public record CreateSchedulingPeriodRequest(
    string Name,
    DateTime FromDate,
    DateTime ToDate);
