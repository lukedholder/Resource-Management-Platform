using System.ComponentModel.DataAnnotations;

namespace ResourcePlatform.Web.Contracts;


public record RegisterRequest(
    [property: Required, EmailAddress, MaxLength(256)] string Email,
    [property: Required, MaxLength(200)] string DisplayName,
    [property: Required, MinLength(8), MaxLength(128)] string Password);

public record LoginRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password);

public record CurrentUserResponse(
    Guid Id,
    string Email,
    string DisplayName);