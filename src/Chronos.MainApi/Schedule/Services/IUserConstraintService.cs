using Chronos.Domain.Schedule;

namespace Chronos.MainApi.Schedule.Services;

public interface IUserConstraintService
{
    Task<Guid> CreateUserConstraintAsync(Guid organizationId, Guid userId, Guid schedulingPeriodId, string key,  string value);
    Task<UserConstraint> GetUserConstraintByIdAsync(Guid organizationId, Guid userConstraintId);
    Task<List<UserConstraint>> GetAllUserConstraintsAsync(Guid organizationId);
    Task<List<UserConstraint>> GetByUserIdAsync(Guid organizationId,  Guid userId);
    Task<List<UserConstraint>> GetBySchedulingPeriodIdAsync(Guid organizationId, Guid schedulingPeriodId);
    Task<List<UserConstraint>> GetBySchedulingPeriodAndUserIdAsync(Guid organizationId, Guid schedulingPeriodId, Guid userId);
    Task UpdateUserConstraintAsync(Guid organizationId, Guid userConstraintId, string key, string value);
    Task DeleteUserConstraintAsync(Guid organizationId, Guid userConstraintId);
}