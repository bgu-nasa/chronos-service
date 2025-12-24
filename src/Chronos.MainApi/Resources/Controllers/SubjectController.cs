using Chronos.MainApi.Resources.Contracts;
using Chronos.MainApi.Resources.Services;
using Chronos.Shared.Exceptions;
using Chronos.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.MainApi.Resources.Controllers;

[ApiController]
[Route("api/resources/subjects/[controller]")]
public class SubjectController(
    ILogger<SubjectController> logger,
    ISubjectService subjectService,
    IActivityService activityService
    ) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateSubjectAsync([FromBody] CreateSubjectRequest request)
    {
        logger.LogInformation("Create subject endpoint was called.");

        var organizationId = GetOrganizationIdFromContext();
        
        var subjectId = await subjectService.CreateSubjectAsync(
            request.OrganizationId,
            request.DepartmentId,
            request.SchedulingPeriodId,
            request.Code,
            request.Name);

        return CreatedAtAction(nameof(GetSubject), new { subjectId }, new { id = subjectId });
    }

    [Authorize]
    [HttpGet("{subjectId}")]
    public async Task<IActionResult> GetSubject(Guid subjectId)
    {
        logger.LogInformation("Get subject endpoint was called for subject {SubjectId}", subjectId);
        var organizationId = GetOrganizationIdFromContext();
        
        var subject = await subjectService.GetSubjectAsync(new Guid(organizationId), subjectId);
        if (subject == null)
            return NotFound();
        
        var subjectResponse = new SubjectResponse(subject.Id);
        return Ok(subjectResponse);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetSubjectsAsync()
    {
        logger.LogInformation("Get subjects endpoint was called.");
        var organizationId = GetOrganizationIdFromContext();
        
        var subjects = await subjectService.GetSubjectsAsync(new Guid(organizationId));
        var subjectResponses = subjects.Select(s => new SubjectResponse(s.Id)).ToList();
        
        return Ok(subjectResponses);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetSubjectsByDepartmentAsync([FromQuery] Guid departmentId)
    {
        logger.LogInformation("Get subjects by department endpoint was called.");
        var organizationId = GetOrganizationIdFromContext();
        
        var subjects = await subjectService.GetSubjectsByDepartmentAsync(new Guid(organizationId), departmentId);
        var subjectResponses = subjects.Select(s => new SubjectResponse(s.Id)).ToList();
        
        return Ok(subjectResponses);
    }
    
    [Authorize]
    [HttpPatch("{subjectId}")]
    public async Task<IActionResult> UpdateSubjectAsync(Guid subjectId, [FromBody] UpdateSubjectRequest request)
    {
        logger.LogInformation("Update subject endpoint was called for subject {SubjectId}", subjectId);
        var organizationId = GetOrganizationIdFromContext();
        
        await subjectService.UpdateSubjectAsync(
            new Guid(organizationId),
            subjectId,
            request.DepartmentId,
            request.SchedulingPeriodId,
            request.Code,
            request.Name);
        
        return NoContent();
    }
    
    [Authorize]
    [HttpDelete("{subjectId}")]
    public async Task<IActionResult> DeleteSubjectAsync(Guid subjectId)
    {
        logger.LogInformation("Delete subject endpoint was called for subject {SubjectId}", subjectId);
        var organizationId = GetOrganizationIdFromContext();
        
        await subjectService.DeleteSubjectAsync(new Guid(organizationId), subjectId);
        
        return NoContent();
    }
    
    [Authorize]
    [HttpPost("{subjectId}/activities")]
    public async Task<IActionResult> CreateActivityAsync(Guid subjectId, [FromBody] CreateActivityRequest request)
    {
        logger.LogInformation("Create activity endpoint was called for subject {SubjectId}", subjectId);
        var organizationId = GetOrganizationIdFromContext();
        
        var activityId =  await activityService.CreateActivityAsync(
            new Guid(organizationId),
            subjectId,
            request.SubjectId,
            request.AssignedUserId,
            request.ActivityType,
            request.ExpectedStudents);
        
        return CreatedAtAction(nameof(GetActivity), new { subjectId, activityId }, new { id =  activityId });
    }
    
    [Authorize]
    [HttpGet("{subjectId}/activities/{activityId}")]
    public async Task<IActionResult> GetActivity(Guid subjectId, Guid activityId)
    {
        logger.LogInformation("Get activity endpoint was called for subject {SubjectId} and activity {ActivityId}", subjectId, activityId);
        var organizationId = GetOrganizationIdFromContext();
        
        var  activity = await activityService.GetActivityAsync(
            new Guid(organizationId),
            activityId);
        
        if (activity == null)
            return NotFound();
        
        var activityResponse = new ActivityResponse(activity.Id);
        return Ok(activityResponse);
    }
    
    [Authorize]
    [HttpGet("activities")]
    public async Task<IActionResult> GetActivitiesAsync()
    {
        logger.LogInformation("Get activities endpoint was called.");
        var organizationId = GetOrganizationIdFromContext();
        
        var activities = await activityService.GetActivitiesAsync(new Guid(organizationId));
        var activityResponses = activities.Select(a => new ActivityResponse(a.Id)).ToList();
        
        return Ok(activityResponses);
    }
    
    [Authorize]
    [HttpGet("{subjectId}/activities")]
    public async Task<IActionResult> GetActivitiesBySubjectAsync(Guid subjectId)
    {
        logger.LogInformation("Get activities by subject endpoint was called for subject {SubjectId}", subjectId);
        var organizationId = GetOrganizationIdFromContext();
        
        var activities = await activityService.GetActivitiesBySubjectAsync(
            new Guid(organizationId),
            subjectId);
        
        var activityResponses = activities.Select(a => new ActivityResponse(a.Id)).ToList();
        
        return Ok(activityResponses);
    }
    
    [Authorize]
    [HttpPatch("activities/{activityId}")]
    public async Task<IActionResult> UpdateActivityAsync(Guid activityId, [FromBody] UpdateActivityRequest request)
    {
        logger.LogInformation("Update activity endpoint was called for activity {ActivityId}", activityId);
        
        var organizationId = GetOrganizationIdFromContext();
        await activityService.UpdateActivityAsync(
            new Guid(organizationId),
            activityId,
            request.SubjectId,
            request.AssignedUserId,
            request.ActivityType,
            request.ExpectedStudents);
        
        return NoContent();
    }
    
    [Authorize]
    [HttpDelete("activities/{activityId}")]
    public async Task<IActionResult> DeleteActivityAsync(Guid activityId)
    {
        logger.LogInformation("Delete activity endpoint was called for activity {ActivityId}", activityId);
        
        var organizationId = GetOrganizationIdFromContext();
        await activityService.DeleteActivityAsync(
            new Guid(organizationId),
            activityId);
        
        return NoContent();
    }
    
    private string GetOrganizationIdFromContext()
    {
        var organizationId = HttpContext.GetOrganizationId();

        if (organizationId is null)
        {
            logger.LogCritical("No organization id was found in the HttpContext, although infra policy requires so.");
            throw new BadRequestException("Missing organization ID in request.");
        }

        return organizationId;
    }
    
}