using System.ComponentModel.DataAnnotations;
using ResourcePlatform.Domain;

namespace ResourcePlatform.Web.Contracts;


public record OrganizationResponse(
    Guid Id,
    string Name,
    string Slug,
    bool IsActive,
    DateTimeOffset CreatedAt);

public record CreateOrganizationRequest(
    [property: Required, MaxLength(OrganizationRules.NameMaxLength)]
    string Name,

    [property: Required, MaxLength(OrganizationRules.SlugMaxLength)]
    [property: RegularExpression(OrganizationRules.SlugPattern,
        ErrorMessage = "Slug must be lowercase letters, digits, and single hyphens.")]
    string Slug);