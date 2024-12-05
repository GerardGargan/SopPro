using Backend.Models.Settings;
using Backend.Service.Interface;
using Microsoft.Extensions.Options;
using PostmarkDotNet;

namespace Backend.Service.Implementation {
    public class EmailService : IEmailService {
        private readonly ApplicationSettings _appSettings;

        public EmailService(IOptions<ApplicationSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body) {
            var client = new PostmarkClient(_appSettings.PostmarkApiToken);

        var message = new PostmarkMessage
        {
            From = _appSettings.PostmarkFromEmail,
            To = toEmail,
            Subject = subject,
            HtmlBody = body
        };

        var response = await client.SendMessageAsync(message);

        return response.Status == PostmarkStatus.Success;
        }

    }

}