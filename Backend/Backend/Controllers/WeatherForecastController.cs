using Backend.Data;
using Backend.Models.DatabaseModels;
using Backend.Models.Settings;
using Backend.Models.Tenancy;
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
        private readonly ApplicationDbContext _dbContext;
        private readonly ITenancyResolver _tenancyResolver;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IOptions<ApplicationSettings> appSettings, IUnitOfWork unitOfWork, IEmailService emailService, ApplicationDbContext dbContext, ITenancyResolver tenancyResolver)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _dbContext = dbContext;
            _tenancyResolver = tenancyResolver;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        //[Authorize]
        public async Task<IEnumerable<Department>> Get()
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
            var tenantId = _tenancyResolver.GetOrganisationid();
            List<string> emails = new List<string>();
            emails.Add("ggargan01@qub.ac.uk");
            //await _emailService.SendEmailAsync(null, emails, "Test", "Testing");
            return await _unitOfWork.Departments.GetAll().ToListAsync();

        }
    }
}
