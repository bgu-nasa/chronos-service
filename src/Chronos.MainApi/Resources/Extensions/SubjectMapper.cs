using Chronos.Domain.Resources;
using Chronos.MainApi.Resources.Contracts;

namespace Chronos.MainApi.Resources.Extensions;

public static class SubjectMapper
{
    public static SubjectResponse ToSubjectResponse(this Subject subject) =>
        new(
            Id: subject.Id,
            OrganizationId: subject.OrganizationId,
            DepartmentId: subject.DepartmentId,
            SchedulingPeriodId: subject.SchedulingPeriodId,
            Code: subject.Code,
            Name: subject.Name
        );
}

