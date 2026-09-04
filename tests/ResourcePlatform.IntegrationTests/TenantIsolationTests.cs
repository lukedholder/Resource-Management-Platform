using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ResourcePlatform.IntegrationTests;


[Collection(nameof(ApiCollection))]
public class TenantIsolationTests(ApiFactory factory)
{
    [Fact]
    public async Task A_member_of_another_organization_cannot_read_my_location()
    {
        var user1 = await TestClient.NewTenantAsync(factory);
        var user2 = await TestClient.NewTenantAsync(factory);

        var locationId = await TestClient.CreateLocationAsync(user1.Client, "User1 HQ");

        var response = await user2.Client.GetAsync($"/api/locations/{locationId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Spoofing_another_organizations_id_in_the_header_is_refused()
    {
        var user1 = await TestClient.NewTenantAsync(factory);
        var user2 = await TestClient.NewTenantAsync(factory);

        var locationId = await TestClient.CreateLocationAsync(user1.Client, "User1 HQ");

        user2.Client.DefaultRequestHeaders.Remove("X-Organization-Id");
        user2.Client.DefaultRequestHeaders.Add("X-Organization-Id", user1.OrganizationId.ToString());

        var response = await user2.Client.GetAsync($"/api/locations/{locationId}");

        Assert.False(response.IsSuccessStatusCode);
        Assert.DoesNotContain("User1 HQ", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Listing_locations_never_includes_another_tenants_rows()
    {
        var user1 = await TestClient.NewTenantAsync(factory);
        var user2 = await TestClient.NewTenantAsync(factory);

        await TestClient.CreateLocationAsync(user1.Client, "User1 Only");

        var body = await user2.Client.GetFromJsonAsync<JsonElement>("/api/locations");

        Assert.Equal(0, body.GetProperty("totalCount").GetInt32());
    }

    [Fact]
    public async Task Anonymous_requests_are_rejected()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/api/locations");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}