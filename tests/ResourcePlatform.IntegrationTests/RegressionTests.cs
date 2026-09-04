using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace ResourcePlatform.IntegrationTests;


[Collection(nameof(ApiCollection))]
public class RegressionTests(ApiFactory factory)
{
    [Fact]
    public async Task Me_is_reachable_with_GET()
    {
        // Was registered with MapPost, so GET returned 405.
        var (client, userId, email) = await TestClient.NewUserAsync(factory);

        var response = await client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(email, body.GetProperty("email").GetString());
    }

    [Fact]
    public async Task Update_and_delete_are_reachable_at_the_id_route()
    {
        // Routes were registered at "/" instead of "/{id:guid}", so these returned 405.
        var tenant = await TestClient.NewTenantAsync(factory);
        var id = await TestClient.CreateLocationAsync(tenant.Client, "Original");

        var put = await tenant.Client.PutAsJsonAsync($"/api/locations/{id}",
            new { name = "Renamed", timeZoneId = "America/Chicago" });
        Assert.Equal(HttpStatusCode.NoContent, put.StatusCode);

        var delete = await tenant.Client.DeleteAsync($"/api/locations/{id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
    }

    [Fact]
    public async Task GetById_returns_the_requested_record_not_merely_the_first_one()
    {
        // GetById was missing its Where clause and returned an arbitrary row.
        // Asking for ONE id is not enough: the arbitrary row might happen to be
        // the one you asked for. Asking for BOTH makes the bug unavoidable,
        // because one unordered query cannot return two different rows.
        var tenant = await TestClient.NewTenantAsync(factory);

        var first = await TestClient.CreateLocationAsync(tenant.Client, "AAA First");
        var second = await TestClient.CreateLocationAsync(tenant.Client, "ZZZ Second");

        var firstBody = await tenant.Client.GetFromJsonAsync<JsonElement>($"/api/locations/{first}");
        var secondBody = await tenant.Client.GetFromJsonAsync<JsonElement>($"/api/locations/{second}");

        Assert.Equal(first, firstBody.GetProperty("id").GetGuid());
        Assert.Equal("AAA First", firstBody.GetProperty("name").GetString());

        Assert.Equal(second, secondBody.GetProperty("id").GetGuid());
        Assert.Equal("ZZZ Second", secondBody.GetProperty("name").GetString());
    }
}