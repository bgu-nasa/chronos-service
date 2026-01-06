namespace Chronos.MainApi.Resources.Contracts;

public sealed record CreateSubjectRequest(
    Guid Id,
    Guid OrganizationId,
    Guid DepartmentId,
    Guid SchedulingPeriodId,
    string Code,
    String Name);