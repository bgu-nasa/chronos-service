namespace Chronos.Domain.Auth;

public class User : ObjectInformation
{
    // Identification & authentication information
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool Verified { get; set; } = false;

    // Profile information
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? AvatarUrl { get; set; }
}
