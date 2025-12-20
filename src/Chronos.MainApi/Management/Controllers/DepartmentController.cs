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
        return Ok(departments);
    }
}