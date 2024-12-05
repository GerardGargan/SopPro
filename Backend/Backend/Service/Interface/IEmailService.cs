namespace Backend.Service.Interface
{
    public interface IEmailService
    {
        public Task<bool> SendEmailAsync(string toEmail, string subject, string body);
    }
}
