namespace ResourcePlatform.Domain;

public class Location : ITenantEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();

    public Guid OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public required string Name { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    public required string TimeZoneId { get; set; }

    public ICollection<Resource> Resources { get; set; } = [];
}