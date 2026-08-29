using System.ComponentModel.DataAnnotations;

namespace ResourcePlatform.Web.Contracts;


public record OrganizationResponse(
    Guid Id,
    string Name,
    string Slug,
    bool IsActive,
    DateTimeOffset CreatedAt);

public record CreateOrganizationRequest(
    [property: Required, MaxLength(200)]
    string Name,

    [property: Required, MaxLength(200)]
    [property: RegularExpression("^[a-z0-9]+(-[a-z0-9]+)*$",
        ErrorMessage = "Slug must be lowercase letters, digits, and single hyphens.")]
    string Slug);