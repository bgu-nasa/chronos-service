using System.Security.Claims;
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
        var response = await authService.RegisterAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Creates a new user in the organization.
    /// </summary>
    /// <param name="organizationId">The organization identifier.</param>
    /// <param name="request">The user creation request.</param>
    /// <returns></returns>
    [RequireOrganization]
    [Authorize(Policy = "OrgRole:UserManager")]
    [HttpPost("/{organizationId}/user")]
    public async Task<IActionResult> CreateUser([FromRoute] string organizationId, [FromBody] CreateUserRequest request)
    {
        logger.LogInformation("Create user endpoint was called");
        await authService.CreateUserAsync(organizationId, request);
        return NoContent();
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
        var response = await authService.LoginAsync(request);
        return Ok(response);
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
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await authService.RefreshTokenAsync(userId);
        return Ok(response);
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
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await authService.VerifyTokenAsync(userId);
        return Ok();
    }

    /// <summary>
    /// Updates the password of the authenticated user.
    /// </summary>
    /// <param name="request">The password update request containing old and new passwords.</param>
    /// <returns>204 No Content when the password is successfully updated.</returns>
    [Authorize]
    [HttpPut("password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UserPasswordUpdateRequest request)
    {
        logger.LogInformation("Update password endpoint was called");
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await authService.UpdatePasswordAsync(userId, request);
        return NoContent();
    }
}