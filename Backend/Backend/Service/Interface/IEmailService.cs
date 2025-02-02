namespace Backend.Service.Interface
{
    public interface IEmailService
    {
        public Task<bool> SendEmailAsync(string toEmail, string subject, string body);
        public Task<bool> SendEmailAsync(List<string> recipients, List<string> bccRecipients, string subject, string body);
        public Task<bool> SendEmailWithPdfAttachmentAsync(List<string> recipients, List<string> bccRecipients, string subject, string body, byte[] pdfData);

    }
}
