namespace ResourcePlatform.Domain;

public class Organization
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public required string Name { get; set; }
    public required string Slug { get; set; }

    public Guid CreatedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsActive { get; set; } = true;

    public ICollection<OrganizationMembership> Memberships { get; set; } = [];

    public ICollection<Department> Departments { get; set; } = [];
    public ICollection<Location> Locations { get; set; } = [];
    public ICollection<ResourceType> ResourceTypes { get; set; } = [];
    public ICollection<Resource> Resources { get; set; } = [];
}