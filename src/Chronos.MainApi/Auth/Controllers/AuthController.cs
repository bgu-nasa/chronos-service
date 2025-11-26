using Chronos.MainApi.Auth.Contracts;
using Chronos.MainApi.Auth.Services;
using Chronos.MainApi.Shared.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.MainApi.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ILogger<AuthController> logger, IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Registers a new customer to the service.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <returns>AuthResponse</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        logger.LogInformation("Register user endpoint was called");
        
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Creates a new user in the organization.
    /// </summary>
    /// <param name="organizationId">The organization identifier.</param>
    /// <param name="request">The user creation request.</param>
    /// <returns></returns>
    [Authorize]
    [RequireOrganization]
    [HttpPost("/{organizationId}/user")]
    public async Task<IActionResult> CreateUser([FromRoute] string organizationId, [FromBody] CreateUserRequest request)
    {
        logger.LogInformation("Create user endpoint was called");
        
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// The login endpoint, call this endpoint in order to authenticate for the service.
    /// </summary>
    /// <param name="request"></param>
    /// <returns>AuthResponse</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        logger.LogInformation("Login user endpoint was called");
        
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Refreshes the token of the user with a new one.
    /// </summary>
    /// <returns>AuthResponse</returns>
    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        logger.LogInformation("Refresh token endpoint was called");
        
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Verifies the token of the user.
    /// </summary>
    /// <returns>200 When the token is valid and not expired.</returns>
    [Authorize]
    [HttpPost("verify")]
    public async Task<IActionResult> Verify()
    {
        logger.LogInformation("Verify token endpoint was called");
        
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
}