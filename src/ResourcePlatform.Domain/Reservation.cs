namespace ResourcePlatform.Domain;

public class Reservation : ITenantEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public Guid ResourceId { get; set; }
    public Resource? Resource { get; set; }

    public Guid CreatedByUserId { get; set; }
    public Guid? ApprovedByUserId { get; set; }

    public DateTimeOffset StartUtc { get; set; }
    public DateTimeOffset EndUtc { get; set; }

    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? Purpose { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}