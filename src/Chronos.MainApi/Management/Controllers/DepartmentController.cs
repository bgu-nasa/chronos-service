using Chronos.MainApi.Management.Extensions;
using Chronos.MainApi.Management.Services;
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
}