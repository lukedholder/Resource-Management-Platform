using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ResourcePlatform.IntegrationTests;


[Collection(nameof(ApiCollection))]
public class AuthorizationTests(ApiFactory factory)
{
    [Fact]
    public async Task Promoting_a_member_changes_their_permissions_without_a_new_login()
    {
        var owner = await TestClient.NewTenantAsync(factory);
        var (memberClient, memberId, memberEmail) = await TestClient.NewUserAsync(factory);

        await owner.Client.PostAsJsonAsync("/api/members", new { email = memberEmail, role = "Viewer" });
        memberClient.DefaultRequestHeaders.Add("X-Organization-Id", owner.OrganizationId.ToString());

        var before = await memberClient.PostAsJsonAsync("/api/locations",
            new { name = "Before", timeZoneId = "America/Chicago" });
        Assert.Equal(HttpStatusCode.Forbidden, before.StatusCode);

        var promote = await owner.Client.PutAsJsonAsync($"/api/members/{memberId}/role",
            new { role = "Administrator" });
        promote.EnsureSuccessStatusCode();

        // Same client, same cookie, no re-login.
        var after = await memberClient.PostAsJsonAsync("/api/locations",
            new { name = "After", timeZoneId = "America/Chicago" });
        Assert.Equal(HttpStatusCode.Created, after.StatusCode);
    }
}