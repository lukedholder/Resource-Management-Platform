namespace ResourcePlatform.Domain;

public class Role
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public Guid? OrganizationId { get; set; }

    public required string Name { get; set; }
    public bool IsSystemRole { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
    public ICollection<OrganizationMembership> Memberships { get; set; } = [];
}