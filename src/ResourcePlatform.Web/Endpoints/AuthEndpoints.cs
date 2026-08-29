using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using ResourcePlatform.Infrastructure;
using ResourcePlatform.Web.Contracts;

namespace ResourcePlatform.Web.Endpoints;


public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", Register).WithName("Register");
        group.MapPost("/login", Login).WithName("Login");
        group.MapPost("/logout", Logout).WithName("Logout").RequireAuthorization();
        group.MapGet("/me", Me).WithName("Me").RequireAuthorization();

        return group;
    }

    private static async Task<Results<Created<CurrentUserResponse>, ValidationProblem>> Register(
        RegisterRequest request,
        UserManager<ApplicationUser> userManager)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName
        };

        var result = await userManager.CreateAsync(user, request.Password); // hashes the password and inserts the row

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .GroupBy(e => e.Code.Contains("Password") ? nameof(request.Password) : nameof(request.Email))
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

            return TypedResults.ValidationProblem(errors);
        }

        var response = new CurrentUserResponse(user.Id, user.Email, user.DisplayName);
        return TypedResults.Created($"/api/users/{user.Id}", response);
    }

    private static async Task<Results<Ok<CurrentUserResponse>, UnauthorizedHttpResult>> Login(
        LoginRequest request,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null) return TypedResults.Unauthorized();

        var result = await signInManager.PasswordSignInAsync(   // verifies the hash and issues the cookie
            user, request.Password, isPersistent: true, lockoutOnFailure: true);

        if (!result.Succeeded) return TypedResults.Unauthorized();

        user.LastLoginAt = DateTimeOffset.UtcNow;
        await userManager.UpdateAsync(user);

        return TypedResults.Ok(new CurrentUserResponse(user.Id, user.Email!, user.DisplayName));
    }

    private static async Task<NoContent> Logout(SignInManager<ApplicationUser> signInManager)
    {
        await signInManager.SignOutAsync();
        return TypedResults.NoContent();
    }

    // Use UserManager to hit the database and return current data
    private static async Task<Results<Ok<CurrentUserResponse>, NotFound>> Me(
        UserManager<ApplicationUser> userManager,
        HttpContext http)
    {
        var user = await userManager.GetUserAsync(http.User);
        return user is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(new CurrentUserResponse(user.Id, user.Email!, user.DisplayName));
    }
}