using Chronos.Domain.Resources;
namespace Chronos.MainApi.Schedule.Services;

public interface IExternalSubjectService
{
    Task<Subject> GetSubjectAsync(Guid organizationId, Guid subjectId);
}
