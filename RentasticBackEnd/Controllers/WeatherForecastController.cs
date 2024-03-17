
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RentasticBackEnd.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly CarRentalContext _context;
        //private readonly CarRentalContext _context;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, CarRentalContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Users/")]
        public IActionResult GetUsers()
        {
            //.Include(u => u.Reservations).Include(u => u.Reviews)
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };

            var users = _context.Users
                .Include(u => u.Reviews)
                .Include(u => u.FavoriteCars)
                .Include(u => u.Reservations)
                .ToList();

            var serializedUsers = JsonSerializer.Serialize(users, options);

            return Ok(serializedUsers);
        }


    }
}
