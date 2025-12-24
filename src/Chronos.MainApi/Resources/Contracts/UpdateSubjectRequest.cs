namespace Chronos.MainApi.Resources.Contracts;

public sealed record UpdateSubjectRequest(
    Guid Id,
    Guid OrganizationId,
    Guid DepartmentId,
    Guid SchedulingPeriodId,
    string Code,
    string Name);