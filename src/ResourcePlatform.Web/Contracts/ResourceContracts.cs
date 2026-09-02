using System.ComponentModel.DataAnnotations;
using ResourcePlatform.Domain;

namespace ResourcePlatform.Web.Contracts;


public record ResourceResponse(
    Guid Id,
    Guid OrganizationId,
    Guid ResourceTypeId,
    Guid? LocationId,
    Guid? DepartmentId,
    string Name,
    string? Description,
    string? AssetTag,
    ResourceStatus Status,
    bool IsReservable,
    bool RequiresApproval);

public record CreateResourceRequest(
    [property: Required] Guid ResourceTypeId,
    Guid? LocationId,
    [property: Required, MaxLength(200)] string Name,
    [property: MaxLength(2000)] string? Description,
    [property: MaxLength(100)] string? AssetTag,
    bool IsReservable = true,
    bool RequiresApproval = false);

public record UpdateResourceRequest(
    [property: Required] Guid ResourceTypeId,
    Guid? LocationId,
    [property: Required, MaxLength(200)] string Name,
    [property: MaxLength(2000)] string? Description,
    [property: MaxLength(100)] string? AssetTag,
    [property: Required] ResourceStatus Status,
    bool IsReservable,
    bool RequiresApproval);