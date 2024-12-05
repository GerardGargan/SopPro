using Backend.Models.DatabaseModels;
using Backend.Models.Settings;
using Backend.Repository.Interface;
using Backend.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IEmailService _emailService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IOptions<ApplicationSettings> appSettings, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        //[Authorize]
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
            //await _emailService.SendEmailAsync("ggargan01@qub.ac.uk", "Test api email", "Testing 123");
            List<string> userIds = new List<string>();
            userIds.Add("bf31a459-93d7-4135-89b8-2068e49d1fea");
            await _emailService.SendEmailAsync(null, userIds, "Test", "Testing");
            return await _unitOfWork.ApplicationUsers.GetAll().ToListAsync();

        }
    }
}
