using Chronos.Data.Repositories.Schedule;
using Chronos.Domain.Schedule;

namespace Chronos.MainApi.Schedule.Services;

public class AssignmentService(
    IAssignmentRepository assignmentRepository,
    ScheduleValidationService validationService,
    ILogger<AssignmentService> logger) : IAssignmentService
{
    public async Task<Guid> CreateAssignmentAsync(Guid organizationId, Guid slotId, Guid resourceId, Guid schedulingItemId)
    {
        logger.LogInformation(
            "Creating assignment. OrganizationId: {OrganizationId}, SlotId: {SlotId}, ResourceId: {ResourceId}, SchedulingItemId: {SchedulingItemId}",
            organizationId, slotId, resourceId, schedulingItemId);
        
        await validationService.ValidateOrganizationAsync(organizationId);
        
        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            SlotId = slotId,
            ResourceId = resourceId,
            ScheduledItemId = schedulingItemId,
        };
        
        await assignmentRepository.AddAsync(assignment);
        
        logger.LogInformation(
            "Assignment created successfully. AssignmentId: {AssignmentId}, OrganizationId: {OrganizationId}",
            assignment.Id, organizationId);
        
        return assignment.Id;
    }

    public async Task<Assignment> GetAssignmentAsync(Guid organizationId, Guid assignmentId)
    {
        logger.LogInformation(
            "Retrieving assignment. OrganizationId: {OrganizationId}, AssignmentId: {AssignmentId}",
            organizationId, assignmentId);
        
        var assignment = await validationService.ValidateAndGetAssignmentAsync(organizationId, assignmentId);
        
        logger.LogInformation(
            "Assignment retrieved successfully. AssignmentId: {AssignmentId}, OrganizationId: {OrganizationId}",
            assignment.Id, organizationId);
        
        return assignment;
    }
    
    public async Task<List<Assignment>> GetAllAssignmentsAsync(Guid organizationId)
    {
        logger.LogInformation("Retrieving all assignments for organization. OrganizationId: {OrganizationId}", organizationId);
        
        await validationService.ValidateOrganizationAsync(organizationId);
        var all = await assignmentRepository.GetAllAsync();
        var filtered = all
            .Where(a => a.OrganizationId == organizationId)
            .ToList();
        
        logger.LogInformation("Retrieved {Count} assignments for organization. OrganizationId: {OrganizationId}", filtered.Count, organizationId);
        return filtered;
    }
    
    public async Task<List<Assignment>> GetAssignmentsBySlotAsync(Guid organizationId, Guid slotId)
    {
        logger.LogInformation(
            "Retrieving assignments by slot. OrganizationId: {OrganizationId}, SlotId: {SlotId}",
            organizationId, slotId);
        await validationService.ValidateOrganizationAsync(organizationId);
        
        var all = await assignmentRepository.GetBySlotIdAsync(slotId);
        var filtered = all
            .Where(a => a.OrganizationId == organizationId)
            .ToList();
        
        logger.LogInformation("Retrieved {Count} assignments for slot. OrganizationId: {OrganizationId}, SlotId: {SlotId}", filtered.Count, organizationId, slotId);
        return filtered;
    }
    
    public async Task<List<Assignment>> GetAssignmentsByScheduledItemAsync(Guid organizationId, Guid schedulingItemId)
    {
        logger.LogInformation(
            "Retrieving assignments by scheduling item. OrganizationId: {OrganizationId}, SchedulingItemId: {SchedulingItemId}",
            organizationId, schedulingItemId);
        await validationService.ValidateOrganizationAsync(organizationId);
        
        var all = await assignmentRepository.GetBySchedulingItemIdAsync(schedulingItemId);
        var filtered = all
            .Where(a => a.OrganizationId == organizationId)
            .ToList();
        
        logger.LogInformation("Retrieved {Count} assignments for scheduling item. OrganizationId: {OrganizationId}, SchedulingItemId: {SchedulingItemId}", filtered.Count, organizationId, schedulingItemId);
        return filtered;
    }
    
    public async Task<Assignment?> GetAssignmentBySlotAndScheduledItemAsync(Guid organizationId, Guid slotId, Guid schedulingItemId)
    {
        logger.LogInformation(
            "Retrieving assignment by slot and scheduling item. OrganizationId: {OrganizationId}, SlotId: {SlotId}, SchedulingItemId: {SchedulingItemId}",
            organizationId, slotId, schedulingItemId);
        await validationService.ValidateOrganizationAsync(organizationId);
        
        var assignment = await assignmentRepository.GetbySlotIdAndSchedulingItemIdAsync(slotId, schedulingItemId);
        if (assignment == null || assignment.OrganizationId != organizationId)
        {
            logger.LogWarning(
                "Assignment not found by slot and scheduling item. OrganizationId: {OrganizationId}, SlotId: {SlotId}, SchedulingItemId: {SchedulingItemId}",
                organizationId, slotId, schedulingItemId);
            throw new KeyNotFoundException("Assignment not found.");
        }
        
        logger.LogInformation(
            "Assignment retrieved successfully by slot and scheduling item. AssignmentId: {AssignmentId}, OrganizationId: {OrganizationId}",
            assignment.Id, organizationId);
        
        return assignment;
    }
    
    public async Task UpdateAssignmentAsync(Guid organizationId, Guid assignmentId, Guid slotId, Guid resourceId, Guid schedulingItemId)
    {
        logger.LogInformation(
            "Updating assignment. OrganizationId: {OrganizationId}, AssignmentId: {AssignmentId}",
            organizationId, assignmentId);
        
        var assignment = await validationService.ValidateAndGetAssignmentAsync(organizationId, assignmentId);
        
        assignment.SlotId = slotId;
        assignment.ResourceId = resourceId;
        assignment.ScheduledItemId = schedulingItemId;
        
        await assignmentRepository.UpdateAsync(assignment);
        
        logger.LogInformation(
            "Assignment updated successfully. AssignmentId: {AssignmentId}, OrganizationId: {OrganizationId}",
            assignment.Id, organizationId);
    }
    
    
    public async Task DeleteAssignmentAsync(Guid organizationId, Guid assignmentId)
    {
        logger.LogInformation(
            "Deleting assignment. OrganizationId: {OrganizationId}, AssignmentId: {AssignmentId}",
            organizationId, assignmentId);
        
        var assignment = await validationService.ValidateAndGetAssignmentAsync(organizationId, assignmentId);
        await assignmentRepository.DeleteAsync(assignment);
        
        logger.LogInformation(
            "Assignment deleted successfully. AssignmentId: {AssignmentId}, OrganizationId: {OrganizationId}",
            assignment.Id, organizationId);
    }
    
    
    
}