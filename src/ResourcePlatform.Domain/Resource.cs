namespace ResourcePlatform.Domain;

public class Resource : ITenantEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public Guid ResourceTypeId { get; set; }
    public ResourceType? ResourceType { get; set; }

    public Guid? LocationId { get; set; }
    public Location? Location { get; set; }

    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? AssetTag { get; set; }

    public ResourceStatus Status { get; set; } = ResourceStatus.Available;
    public bool IsReservable { get; set; } = true;
    public bool RequiresApproval { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Reservation> Reservations { get; set; } = [];
}