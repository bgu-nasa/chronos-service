using Chronos.Domain.Schedule;

namespace Chronos.MainApi.Schedule.Services;

public interface IAssignmentService
{
    Task<Guid> CreateAssignmentAsync(Guid organizationId, Guid slotId, Guid resourceId, Guid ScheduledItemId);
    
    Task<Assignment> GetAssignmentAsync(Guid organizationId, Guid assignmentId);
    
    Task<List<Assignment>> GetAllAssignmentsAsync(Guid organizationId);
    
    Task<List<Assignment>> GetAssignmentsBySlotAsync(Guid organizationId, Guid slotId);
    
    Task<List<Assignment>> GetAssignmentsByScheduledItemAsync(Guid organizationId, Guid scheduledItemId);
    
    Task<Assignment?> GetAssignmentBySlotAndResourceItemAsync(Guid organizationId, Guid slotId, Guid resourceId);
    
    Task UpdateAssignmentAsync(Guid organizationId, Guid assignmentId, Guid slotId, Guid resourceId , Guid scheduledItemId);
    
    Task DeleteAssignmentAsync(Guid organizationId, Guid assignmentId);
    
    
}