using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

public class CoffeeMachineController : ControllerBase
{
    private static int _counter = 0;

    [HttpGet("brew-coffee")]
    public async Task<IActionResult> BrewCoffee(string location)
    {
        var now = DateTime.UtcNow;
        if (now.Month == 4 && now.Day == 1)
        {
            return StatusCode(418);
        }

        if (++_counter % 5 == 0)
        {
            return StatusCode(503);
        }

        double temperature = await GetCurrentTemperature(location);
        var message = temperature > 30 ? "Your refreshing iced coffee is ready" : "Your piping hot coffee is ready";

        return Ok(new
        {
            message,
            prepared = now.ToString("o")
        });
    }

    private async Task<double> GetCurrentTemperature(string location)
    {
        // Example implementation using OpenWeatherMap API
        var client = new HttpClient();
        var result = await client.GetAsync($"http://api.openweathermap.org/data/2.5/weather?q={location}&appid=YOUR_APP_ID");
        result.EnsureSuccessStatusCode();

        var content = await result.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject<dynamic>(content);
        return data.main.temp - 273.15;
    }
}
