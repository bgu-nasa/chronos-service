namespace Chronos.MainApi.Schedule.Contracts;

public record OrganizationPolicyResponse(
    string Id,
    string OrganizationId,
    string SchedulingPeriodId,
    string Key,
    string Value);
