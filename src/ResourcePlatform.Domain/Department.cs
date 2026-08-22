namespace ResourcePlatform.Domain;

public class Department : ITenantEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Resource> Resources { get; set; } = [];
}