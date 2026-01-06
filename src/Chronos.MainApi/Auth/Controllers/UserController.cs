using Chronos.MainApi.Auth.Contracts;
using Chronos.MainApi.Auth.Services;
using Chronos.MainApi.Shared.Controllers.Utils;
using Chronos.MainApi.Shared.Extensions;
using Chronos.MainApi.Shared.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.MainApi.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
[RequireOrganization]
[Authorize]
public class UserController(
    ILogger<UserController> logger,
    IUserService userService,
    IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Creates a new user in the organization.
    /// </summary>
    /// <param name="request">The user creation request.</param>
    /// <returns>201 Created with the new user information.</returns>
    [HttpPost]
    [Authorize(Policy = "OrgRole:UserManager")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        logger.LogInformation("Create user endpoint was called for organization {OrganizationId}", organizationId);
        var response = await authService.CreateUserAsync(organizationId.ToString(), request);
        return CreatedAtAction(nameof(GetUser), new { userId = response.UserId }, response);
    }

    /// <summary>
    /// Gets a specific user by ID within an organization.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>The user details.</returns>
    [HttpGet("{userId}")]
    [Authorize(Policy = "OrgRole:UserManager")]
    public async Task<IActionResult> GetUser([FromRoute] Guid userId)
    {
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        logger.LogInformation("Get user {UserId} from organization {OrganizationId}", userId, organizationId);
        var response = await userService.GetUserAsync(organizationId, userId);
        return Ok(response);
    }

    /// <summary>
    /// Gets all users within an organization.
    /// </summary>
    /// <returns>A list of users in the organization.</returns>
    [HttpGet]
    [Authorize(Policy = "OrgRole:UserManager")]
    public async Task<IActionResult> GetUsers()
    {
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        logger.LogInformation("Get all users from organization {OrganizationId}", organizationId);
        var response = await userService.GetUsersAsync(organizationId);
        return Ok(response);
    }

    /// <summary>
    /// Updates a user's profile information.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="request">The user update request containing profile information.</param>
    /// <returns>204 No Content when the profile is successfully updated.</returns>
    [HttpPut("{userId}")]
    [Authorize(Policy = "OrgRole:UserManager")]
    public async Task<IActionResult> UpdateUserProfile(
        [FromRoute] Guid userId,
        [FromBody] UserUpdateRequest request)
    {
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        logger.LogInformation("Update user {UserId} profile in organization {OrganizationId}", userId, organizationId);
        await userService.UpdateUserProfileAsync(organizationId, userId, request);
        return NoContent();
    }

    /// <summary>
    /// Updates the authenticated user's own profile information.
    /// </summary>
    /// <param name="request">The user update request containing profile information.</param>
    /// <returns>204 No Content when the profile is successfully updated.</returns>
    [HttpPut]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UserUpdateRequest request)
    {
        var userId = User.GetUserId();
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        logger.LogInformation("User {UserId} updating own profile in organization {OrganizationId}", userId, organizationId);
        await userService.UpdateUserProfileAsync(organizationId, userId, request);
        return NoContent();
    }

    /// <summary>
    /// Deletes a user from the organization.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>204 No Content when the user is successfully deleted.</returns>
    [HttpDelete("{userId}")]
    [Authorize(Policy = "OrgRole:UserManager")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
    {
        var organizationId = ControllerUtils.GetOrganizationIdAndFailIfMissing(HttpContext, logger);
        logger.LogInformation("Delete user {UserId} from organization {OrganizationId}", userId, organizationId);
        await userService.DeleteUserAsync(organizationId, userId);
        return NoContent();
    }
}
