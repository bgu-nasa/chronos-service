using Chronos.Domain.Resources;
using Chronos.MainApi.Resources.Services;

namespace Chronos.MainApi.Schedule.Services;

public class ExternalSubjectService(
    ISubjectService subjectService
) : IExternalSubjectService
{
    public async Task<Subject> GetSubjectAsync(Guid organizationId, Guid subjectId)
    {
        return await subjectService.GetSubjectAsync(organizationId, subjectId);
    }

    public async Task<List<Subject>> GetAllSubjectsBySchedulingPeriodAync(Guid organizationId, Guid schedulingPeriodId)
    {
        var subjects = await subjectService.GetSubjectsAsync(organizationId);
        return subjects.Where(s => s.SchedulingPeriodId == schedulingPeriodId).ToList();
    }
    public async Task DeleteSubjectAsync(Guid organizationId, Guid subjectId)
    {
        await subjectService.DeleteSubjectAsync(organizationId, subjectId);
    }
}
