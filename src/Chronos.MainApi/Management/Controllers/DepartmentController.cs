using Chronos.MainApi.Management.Services;
using Chronos.MainApi.Shared.Middleware;
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
    // Create, list, update, delete & undelete.
}