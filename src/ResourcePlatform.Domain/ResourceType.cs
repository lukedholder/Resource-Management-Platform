namespace ResourcePlatform.Domain;

public class ResourceType : ITenantEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; }

    public ICollection<Resource> Resources { get; set; } = [];
}