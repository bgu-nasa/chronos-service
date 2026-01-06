using Chronos.MainApi.Resources.Contracts;
using Chronos.MainApi.Resources.Extensions;
using Chronos.MainApi.Resources.Services;
using Chronos.Shared.Exceptions;
using Chronos.Shared.Extensions;
using Chronos.MainApi.Shared.Controllers.ControllerUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.MainApi.Resources.Controllers;

[ApiController]
[Route("api/department/{departmentId}/resources/subjects/[controller]")]
public class SubjectController(
    ILogger<SubjectController> logger,
    ISubjectService subjectService,
    IActivityService activityService
    ) : ControllerBase
{
    [Authorize(Policy = "OrgRole:ResourceManager")]
    [HttpPost]
    public async Task<IActionResult> CreateSubjectAsync([FromBody] CreateSubjectRequest request)
    {
        logger.LogInformation("Create subject endpoint was called.");

        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        
        var subjectId = await subjectService.CreateSubjectAsync(
            request.OrganizationId,
            request.DepartmentId,
            request.SchedulingPeriodId,
            request.Code,
            request.Name);

        var subject = await subjectService.GetSubjectAsync(organizationId, subjectId);
        var response = subject.ToSubjectResponse();

        return CreatedAtAction(nameof(GetSubject), new { subjectId }, response);
    }

    [Authorize(Policy = "OrgRole:Viewer")]
    [HttpGet("{subjectId}")]
    public async Task<IActionResult> GetSubject(Guid subjectId)
    {
        logger.LogInformation("Get subject endpoint was called for subject {SubjectId}", subjectId);
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        
        var subject = await subjectService.GetSubjectAsync(organizationId, subjectId);
        if (subject == null)
            return NotFound();
        
        var subjectResponse = subject.ToSubjectResponse();
        return Ok(subjectResponse);
    }
    
    [Authorize(Policy = "OrgRole:Viewer")]
    [HttpGet]
    public async Task<IActionResult> GetSubjectsAsync()
    {
        logger.LogInformation("Get subjects endpoint was called.");
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        
        var subjects = await subjectService.GetSubjectsAsync(organizationId);
        var subjectResponses = subjects.Select(s => s.ToSubjectResponse()).ToList();
        
        return Ok(subjectResponses);
    }
    
    [Authorize(Policy = "OrgRole:Viewer")]
    [HttpGet]
    public async Task<IActionResult> GetSubjectsByDepartmentAsync([FromQuery] Guid departmentId)
    {
        logger.LogInformation("Get subjects by department endpoint was called.");
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        
        var subjects = await subjectService.GetSubjectsByDepartmentAsync(organizationId, departmentId);
        var subjectResponses = subjects.Select(s => s.ToSubjectResponse()).ToList();
        
        return Ok(subjectResponses);
    }
    
    [Authorize(Policy = "OrgRole:ResourceManager")]
    [HttpPatch("{subjectId}")]
    public async Task<IActionResult> UpdateSubjectAsync(Guid subjectId, [FromBody] UpdateSubjectRequest request)
    {
        logger.LogInformation("Update subject endpoint was called for subject {SubjectId}", subjectId);
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        
        await subjectService.UpdateSubjectAsync(
            organizationId,
            subjectId,
            request.DepartmentId,
            request.SchedulingPeriodId,
            request.Code,
            request.Name);
        
        return NoContent();
    }
    
    [Authorize(Policy = "OrgRole:ResourceManager")]
    [HttpDelete("{subjectId}")]
    public async Task<IActionResult> DeleteSubjectAsync(Guid subjectId)
    {
        logger.LogInformation("Delete subject endpoint was called for subject {SubjectId}", subjectId);
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        
        await subjectService.DeleteSubjectAsync(organizationId, subjectId);
        
        return NoContent();
    }
    
    [Authorize(Policy = "OrgRole:ResourceManager")]
    [HttpPost("{subjectId}/activities")]
    public async Task<IActionResult> CreateActivityAsync(Guid subjectId, [FromBody] CreateActivityRequest request)
    {
        logger.LogInformation("Create activity endpoint was called for subject {SubjectId}", subjectId);
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        
        var activityId =  await activityService.CreateActivityAsync(
            organizationId,
            subjectId,
            request.SubjectId,
            request.AssignedUserId,
            request.ActivityType,
            request.ExpectedStudents);
        
        var activity = await activityService.GetActivityAsync(organizationId, activityId);
        var response = activity.ToActivityResponse();
        
        return CreatedAtAction(nameof(GetActivity), new { subjectId, activityId }, response);
    }
    
    [Authorize(Policy = "OrgRole:Viewer")]
    [HttpGet("{subjectId}/activities/{activityId}")]
    public async Task<IActionResult> GetActivity(Guid subjectId, Guid activityId)
    {
        logger.LogInformation("Get activity endpoint was called for subject {SubjectId} and activity {ActivityId}", subjectId, activityId);
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        
        var  activity = await activityService.GetActivityAsync(
            organizationId,
            activityId);
        
        if (activity == null)
            return NotFound();
        
        var activityResponse = activity.ToActivityResponse();
        return Ok(activityResponse);
    }
    
    [Authorize(Policy = "OrgRole:Viewer")]
    [HttpGet("activities")]
    public async Task<IActionResult> GetActivitiesAsync()
    {
        logger.LogInformation("Get activities endpoint was called.");
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        
        var activities = await activityService.GetActivitiesAsync(organizationId);
        var activityResponses = activities.Select(a => a.ToActivityResponse()).ToList();
        
        return Ok(activityResponses);
    }
    
    [Authorize(Policy = "OrgRole:Viewer")]
    [HttpGet("{subjectId}/activities")]
    public async Task<IActionResult> GetActivitiesBySubjectAsync(Guid subjectId)
    {
        logger.LogInformation("Get activities by subject endpoint was called for subject {SubjectId}", subjectId);
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        
        var activities = await activityService.GetActivitiesBySubjectAsync(
            organizationId,
            subjectId);
        
        var activityResponses = activities.Select(a => a.ToActivityResponse()).ToList();
        
        return Ok(activityResponses);
    }
    
    [Authorize(Policy = "OrgRole:ResourceManager")]
    [HttpPatch("activities/{activityId}")]
    public async Task<IActionResult> UpdateActivityAsync(Guid activityId, [FromBody] UpdateActivityRequest request)
    {
        logger.LogInformation("Update activity endpoint was called for activity {ActivityId}", activityId);

        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        await activityService.UpdateActivityAsync(
            organizationId,
            activityId,
            request.SubjectId,
            request.AssignedUserId,
            request.ActivityType,
            request.ExpectedStudents);
        
        return NoContent();
    }
    
    [Authorize(Policy = "OrgRole:ResourceManager")]
    [HttpDelete("activities/{activityId}")]
    public async Task<IActionResult> DeleteActivityAsync(Guid activityId)
    {
        logger.LogInformation("Delete activity endpoint was called for activity {ActivityId}", activityId);
        
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        await activityService.DeleteActivityAsync(organizationId, activityId);
        
        return NoContent();
    }
    
}