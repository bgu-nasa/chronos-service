using Microsoft.AspNetCore.Mvc;

namespace Chronos.MainApi.Shared.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController(ILogger<HealthController> logger) : ControllerBase
{
    private record HealthResponse(DateTime ResponseTime, string Message, string ServiceInstance);
    
    [HttpGet]
    public IActionResult GetHealthStatus()
    {
        logger.LogInformation("Health check requested at {Time}", DateTime.UtcNow);

        // Simple hardcoded health report
        var response = new HealthResponse(
            DateTime.UtcNow,
            "Service is healthy",
            Environment.MachineName
        );

        return Ok(response);
    }
}