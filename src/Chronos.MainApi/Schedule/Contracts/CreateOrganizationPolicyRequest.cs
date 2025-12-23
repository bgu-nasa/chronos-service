namespace Chronos.MainApi.Schedule.Contracts;

public record CreateOrganizationPolicyRequest(
    Guid SchedulingPeriodId,
    string Key,
    string Value);
