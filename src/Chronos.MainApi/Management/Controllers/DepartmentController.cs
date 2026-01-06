using Chronos.MainApi.Management.Contracts;
using Chronos.MainApi.Management.Extensions;
using Chronos.MainApi.Management.Services;
using Chronos.MainApi.Shared.Controllers.ControllerUtils;
using Chronos.MainApi.Shared.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.MainApi.Management.Controllers;

[ApiController]
[RequireOrganization]
[Route("api/management/[controller]")]
public class DepartmentController(
    ILogger<DepartmentController> logger,
    IDepartmentService departmentService)
: ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllDepartments()
    {
        logger.LogInformation("Get all departments");
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        var departments = await departmentService.GetDepartmentsAsync(organizationId);
        return Ok(departments.Select(d => d.ToDepartmentResponse()).ToArray());
    }

    [Authorize]
    [HttpGet("{departmentId}")]
    public async Task<IActionResult> GetDepartmentById(string departmentId)
    {
        logger.LogInformation("Get department by id: {deptId}", departmentId);

        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);

        if (!Guid.TryParse(departmentId, out var departmentGuid))
        {
            logger.LogInformation("Invalid format of department ID in request.");
            return BadRequest("Invalid format of department ID in request.");
        }

        var department = await departmentService.GetDepartmentAsync(organizationId, departmentGuid);

        return Ok(department.ToDepartmentResponse());
    }

    [Authorize(Policy = "OrgRole:ResourceManager")]
    [HttpPost]
    public async Task<IActionResult> CreateDepartment([FromBody] DepartmentRequest departmentRequest)
    {
        logger.LogInformation("Create new department with name: {deptName}", departmentRequest.Name);

        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);

        var department = await departmentService.CreateDepartmentAsync(organizationId, departmentRequest.Name);

        return CreatedAtAction(
            nameof(GetDepartmentById),
            new { departmentId = department.Id },
            department.ToDepartmentResponse()
        );
    }

    [Authorize(Policy = "DeptRole:Administrator")]
    [HttpPatch("{departmentId}")]
    public async Task<IActionResult> UpdateDepartment([FromRoute] string departmentId, [FromBody] DepartmentRequest request)
    {
        logger.LogInformation("Update department with id: {deptId}", departmentId);

        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);

        if (!Guid.TryParse(departmentId, out var departmentGuid))
        {
            logger.LogInformation("Invalid format of department ID in request.");
            return BadRequest("Invalid format of department ID in request.");
        }

        await departmentService.UpdateDepartmentAsync(organizationId, departmentGuid, request.Name);

        return NoContent();
    }

    [Authorize(Policy = "DeptRole:Administrator")]
    [HttpDelete("{departmentId}")]
    public async Task<IActionResult> DeleteDepartment([FromRoute] string departmentId)
    {
        logger.LogInformation("Delete department with id: {deptId}", departmentId);
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);

        if (!Guid.TryParse(departmentId, out var departmentGuid))
        {
            logger.LogInformation("Invalid format of department ID in request.");
            return BadRequest("Invalid format of department ID in request.");
        }

        await departmentService.SetForDeletionAsync(organizationId, departmentGuid);

        return NoContent();
    }

    [Authorize(Policy = "DeptRole:Administrator")]
    [HttpPost("restore/{departmentId}")]
    public async Task<IActionResult> RestoreDepartment([FromRoute] string departmentId)
    {
        logger.LogInformation("Delete department with id: {deptId}", departmentId);
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);

        if (!Guid.TryParse(departmentId, out var departmentGuid))
        {
            logger.LogInformation("Invalid format of department ID in request.");
            return BadRequest("Invalid format of department ID in request.");
        }

        await departmentService.RestoreDeletedDepartmentAsync(organizationId, departmentGuid);

        return NoContent();
    }
}