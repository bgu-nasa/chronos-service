namespace Chronos.Domain.Management.Roles;

/// <summary>
/// Houses all the possible role types within the system.
/// </summary>
public enum Role
{
    Administrator, // Can do everything
    UserManager, // Can manage users in the organization
    ResourceManager, // Can manage resources in the specified scope
    Operator, // Can perform scheduling operations and tasks on the scope
    Viewer // Read-only access to the scope
}