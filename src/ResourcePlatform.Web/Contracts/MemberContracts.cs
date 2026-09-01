using System.ComponentModel.DataAnnotations;

namespace ResourcePlatform.Web.Contracts;


public record MemberResponse(
    Guid UserId,
    string Email,
    string DisplayName,
    string Role,
    string Status,
    DateTimeOffset JoinedAt);

public record AddMemberRequest(
    [property: Required, EmailAddress, MaxLength(256)] string Email,
    [property: Required, MaxLength(100)] string Role);

public record UpdateMemberRoleRequest(
    [property: Required, MaxLength(100)] string Role);