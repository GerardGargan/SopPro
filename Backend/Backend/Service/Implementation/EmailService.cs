using Backend.Models.DatabaseModels;
using Backend.Models.Settings;
using Backend.Repository.Interface;
using Backend.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PostmarkDotNet;

namespace Backend.Service.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly ApplicationSettings _appSettings;
        private readonly IUnitOfWork _unitOfWork;

        public EmailService(IOptions<ApplicationSettings> appSettings, IUnitOfWork unitOfWork)
        {
            _appSettings = appSettings.Value;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Sends an email to a single recipient
        /// </summary>
        /// <param name="toEmail"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
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

        /// <summary>
        /// Sends an email to multiple recipients
        /// </summary>
        /// <param name="recipients">The id's of the recipients to send the email to</param>
        /// <param name="bccRecipients">The id's of the bcc recipients</param>
        /// <param name="subject">The email subject</param>
        /// <param name="body">The email body</param>
        /// <returns></returns>
        public async Task<bool> SendEmailAsync(List<string> recipients, List<string> bccRecipients, string subject, string body)
        {

            // return false if there are no recipients
            if ((recipients == null || recipients.Count == 0) && (bccRecipients == null || bccRecipients.Count == 0))
            {
                return false;
            }

            // handle nulls by creating empty lists
            recipients = recipients ?? new List<string>(0);
            bccRecipients = bccRecipients ?? new List<string>(0);

            string toEmails = string.Join(",", recipients);
            string bccEmails = string.Join(",", bccRecipients);

            var client = new PostmarkClient(_appSettings.PostmarkApiToken);

            var message = new PostmarkMessage
            {
                From = _appSettings.PostmarkFromEmail,
                To = toEmails,
                Bcc = bccEmails,
                Subject = subject,
                HtmlBody = body
            };

            try
            {
                var response = await client.SendMessageAsync(message);

                return response.Status == PostmarkStatus.Success;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> SendEmailWithPdfAttachmentAsync(List<string> recipients, List<string> bccRecipients, string subject, string body, byte[] pdfData, string pdfName)
        {
            // return false if there are no recipients
            if ((recipients == null || recipients.Count == 0) && (bccRecipients == null || bccRecipients.Count == 0))
            {
                return false;
            }

            // handle nulls by creating empty lists
            recipients = recipients ?? new List<string>(0);
            bccRecipients = bccRecipients ?? new List<string>(0);

            string toEmails = string.Join(",", recipients);
            string bccEmails = string.Join(",", bccRecipients);

            var client = new PostmarkClient(_appSettings.PostmarkApiToken);

            var message = new PostmarkMessage
            {
                From = _appSettings.PostmarkFromEmail,
                To = toEmails,
                Bcc = bccEmails,
                Subject = subject,
                HtmlBody = body,
                Attachments = new List<PostmarkMessageAttachment>
            {
                new PostmarkMessageAttachment
                {
                    Name = pdfName,
                    Content = Convert.ToBase64String(pdfData),
                    ContentType = "application/pdf"
                }
            }
            };

            try
            {
                var response = await client.SendMessageAsync(message);

                return response.Status == PostmarkStatus.Success;
            }
            catch (Exception)
            {
                return false;
            }

        }

    }

}