using System.ComponentModel.DataAnnotations;
using ResourcePlatform.Domain;

namespace ResourcePlatform.Web.Contracts;


public record ReservationResponse(
    Guid Id,
    Guid ResourceId,
    string ResourceName,
    Guid CreatedByUserId,
    Guid? ApprovedByUserId,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc,
    ReservationStatus Status,
    string? Purpose);

public record CreateReservationRequest(
    [property: Required] Guid ResourceId,
    [property: Required] DateTimeOffset StartUtc,
    [property: Required] DateTimeOffset EndUtc,
    [property: MaxLength(1000)] string? Purpose);