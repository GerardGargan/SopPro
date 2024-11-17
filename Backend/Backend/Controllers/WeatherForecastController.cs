using Backend.Models.DatabaseModels;
using Backend.Models.Settings;
using Backend.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ApplicationSettings _appSettings;


        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IOptions<ApplicationSettings> appSettings, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<ApplicationUser>> Get()
        {
            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            //    TemperatureC = Random.Shared.Next(-20, 55),
            //    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            //})
            //.ToArray();
            //return Ok(_appSettings.JwtSecret);

            //var userToRemove = await _unitOfWork.ApplicationUser.GetAsync(x => x.Email == "user@example.com");
            //_unitOfWork.ApplicationUser.Remove(userToRemove);
            //await _unitOfWork.SaveAsync();
            return await _unitOfWork.ApplicationUsers.GetAllAsync();

        }
    }
}
