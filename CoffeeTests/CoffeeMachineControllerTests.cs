using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

public class CoffeeMachineControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CoffeeMachineControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task BrewCoffee_Returns200Ok_WhenBrewingCoffee()
    {
        var response = await _client.GetAsync("/brew-coffee?location=London,uk");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<dynamic>(content);

        Assert.Equal("Your piping hot coffee is ready", (string)data.message);
        Assert.NotNull((string)data.prepared);
    }

    [Fact]
    public async Task BrewCoffee_Returns418IAmATeapot_OnApril1st()
    {
        var response = await _client.GetAsync("/brew-coffee?location=London,uk");
        Assert.Equal(418, (int)response.StatusCode);
    }

    [Fact]
    public async Task BrewCoffee_Returns503ServiceUnavailable_OnEveryFifthCall()
    {
        var response = new HttpResponseMessage();
        for (int i = 0; i < 5; i++)
        {
            response = await _client.GetAsync("/brew-coffee?location=London,uk");
            response.EnsureSuccessStatusCode();
        }

        response = await _client.GetAsync("/brew-coffee?location=London,uk");
        Assert.Equal(503, (int)response.StatusCode);
    }

    [Fact]
    public async Task BrewCoffee_ReturnsRefreshingIcedCoffee_WhenTemperatureIsGreaterThan30C()
    {
        var response = await _client.GetAsync("/brew-coffee?location=Dubai,ae");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<dynamic>(content);

        Assert.Equal("Your refreshing iced coffee is ready", (string)data.message);
        Assert.NotNull((string)data.prepared);
    }
}
