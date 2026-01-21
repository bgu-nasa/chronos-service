using Chronos.Domain.Resources;
namespace Chronos.MainApi.Schedule.Services;

public interface IExternalSubjectService
{
    Task<Subject> GetSubjectAsync(Guid organizationId, Guid subjectId);
    Task<List<Subject>> GetAllSubjectsBySchedulingPeriodAync(Guid organizationId, Guid schedulingPeriodId);
    Task DeleteSubjectAsync(Guid organizationId, Guid subjectId);
}
