namespace Chronos.Domain.Auth;

public class User : ObjectInformation
{
    // Identification & authentication information
    public Guid Id { get; set; }
    // TODO: Remove OrganizationId - it should be in StaffProfile
    public Guid OrganizationId { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }

    // Profile information
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? AvatarUrl { get; set; }
}
