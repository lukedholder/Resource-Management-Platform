using System.Net.Http.Json;
using System.Text.Json;

namespace ResourcePlatform.IntegrationTests;


/// <summary>
/// An authenticated HttpClient plus the ids it was set up with.
/// </summary>
public sealed record Tenant(HttpClient Client, Guid OrganizationId, Guid UserId, string email);

public static class TestClient
{
    public static string UniqueEmail() => $"u{Guid.NewGuid():N}@example.com";
    public static string UniqueSlug() => $"org-{Guid.NewGuid():N}"[..20];

    // Registers a user, logs in, and returns a cookie-carrying client
    public static async Task<(HttpClient Client, Guid UserId, string Email)> NewUserAsync(ApiFactory factory)
    {
        var client = factory.CreateClient();
        var email = UniqueEmail();

        var register = await client.PostAsJsonAsync("/api/auth/register", new { email, displayName = "Test User", password = "Str0ng!Passw0rd" });

        register.EnsureSuccessStatusCode();

        var created = await register.Content.ReadFromJsonAsync<JsonElement>();
        var userId = created.GetProperty("id").GetGuid();

        var login = await client.PostAsJsonAsync("/api/auth/login", new { email, password = "Str0ng!Passw0rd" });
        login.EnsureSuccessStatusCode();

        return (client, userId, email);
    }

    // A user who owns a brand new organization, with the tenant header set
    public static async Task<Tenant> NewTenantAsync(ApiFactory factory)
    {
        var (client, userId, email) = await NewUserAsync(factory);

        var response = await client.PostAsJsonAsync("/api/organizations", new { name = "Test Org", slug = UniqueSlug() });
        response.EnsureSuccessStatusCode();

        var org = await response.Content.ReadFromJsonAsync<JsonElement>();
        var orgId = org.GetProperty("id").GetGuid();

        client.DefaultRequestHeaders.Add("X-Organization-Id", orgId.ToString());
        return new Tenant(client, orgId, userId, email);
    }

    public static async Task<Guid> CreateLocationAsync(HttpClient client, string name)
    {
        var r = await client.PostAsJsonAsync("/api/locations", new { name, timeZoneId = "America/Chicago" });
        r.EnsureSuccessStatusCode();
        var body = await r.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("id").GetGuid();
    }

    public static async Task<Guid> CreateResourceAsync(HttpClient client, string name)
    {
        var t = await client.PostAsJsonAsync("/api/resource-types", new { name = $"Type {Guid.NewGuid():N}"[..20] });
        t.EnsureSuccessStatusCode();
        var typeId = (await t.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();

        var r = await client.PostAsJsonAsync("/api/resources", new { resourceTypeId = typeId, name });
        r.EnsureSuccessStatusCode();
        return (await r.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();
    }
}