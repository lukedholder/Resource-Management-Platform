using System.ComponentModel.DataAnnotations;

namespace ResourcePlatform.Web.Contracts;


public record LocationResponse(
    Guid Id,
    Guid OrganizationId,
    string Name,
    string? AddressLine1,
    string? City,
    string? State,
    string? PostalCode,
    string TimeSizeId);

public record CreateLocationRequest(
    [property: Required, MaxLength(200)] string Name,
    [property: MaxLength(200)] string? AddressLine1,
    [property: MaxLength(100)] string? City,
    [property: MaxLength(100)] string? State,
    [property: MaxLength(20)] string? PostalCode,
    [property: Required, MaxLength(100)] string TimeZoneId);

public record UpdateLocationRequest(
    [property: Required, MaxLength(200)] string Name,
    [property: MaxLength(200)] string? AddressLine1,
    [property: MaxLength(100)] string? City,
    [property: MaxLength(100)] string? State,
    [property: MaxLength(20)] string? PostalCode,
    [property: Required, MaxLength(100)] string TimeZoneId);