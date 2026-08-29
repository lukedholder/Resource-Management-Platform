using System.ComponentModel.DataAnnotations;

namespace ResourcePlatform.Web.Contracts;


public record ResourceTypeResponse(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string? Description);

public record CreateResourceTypeRequest(
    [property: Required, MaxLength(200)] string Name,
    [property: MaxLength(1000)] string? Description);

public record UpdateResourceTypeRequest(
    [property: Required, MaxLength(200)] string Name,
    [property: MaxLength(1000)] string? Description);