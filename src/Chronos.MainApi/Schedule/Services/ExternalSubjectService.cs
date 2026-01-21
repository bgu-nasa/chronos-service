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
}
