using Chronos.MainApi.Management.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.MainApi.Management.Controllers;

[ApiController]
[Route("api/management/[controller]")]
public class DepartmentController(
    ILogger<DepartmentController> logger,
    IDepartmentService departmentService)
: ControllerBase
{
    // Create, list, update, delete & undelete.
}