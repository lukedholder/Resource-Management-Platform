namespace ResourcePlatform.Domain;

public class OrganizationMembership : ITenantEntity
{
    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }
    public Role? Role { get; set; }

    public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
    public MembershipStatus Status { get; set; } = MembershipStatus.Invited;
}