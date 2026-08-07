using Microsoft.AspNetCore.Mvc;
namespace jobFinderBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    [HttpGet]
    public IEnumerable<WeatherForecastController> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecastController
        {
            
        })
        .ToArray();
    }
}

