namespace Chronos.Domain.Management;

public class StaffProfile : ObjectInformation
{
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
    public required string DisplayName { get; set; }
    public required string Title { get; set; }
    public required string OrganizationEmail { get; set; }
}
