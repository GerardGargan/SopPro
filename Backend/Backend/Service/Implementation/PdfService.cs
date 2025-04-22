using System.Threading.Tasks;
using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Models.Tenancy;
using Backend.Repository.Implementation;
using Backend.Repository.Interface;
using Backend.Service.Interface;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Backend.Service.Implementation
{
    public class PdfService : IPdfService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly ITenancyResolver _tenancyResolver;
        private readonly ITemplateService _templateService;
        public PdfService(IUnitOfWork unitOfWork, IEmailService emailService, ITenancyResolver tenancyResolver, ITemplateService templateService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _tenancyResolver = tenancyResolver;
            _templateService = templateService;
        }

        /// <summary>
        /// Generates a pdf for a specified SOP Version
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<byte[]> GeneratePdf(string templateName, SopVersionDto model)
        {
            // Check if the organisation has a custom logo uploaded
            var settingLogo = await _unitOfWork.Settings.GetAsync(x => x.Key == "logo");
            var customLogoUrl = settingLogo == null ? null : settingLogo.Value;

            QuestPDF.Settings.License = LicenseType.Community;

            byte[] template = null;

            // Determine which template to use and create the pdf
            switch (templateName.ToLower())
            {
                case "template1":
                    template = Template1(model, customLogoUrl);
                    break;
                default:
                    throw new Exception("Invalid template name");
            }

            // send email attachment
            // Fetch the detils of the user logged in for sending the email
            var userId = _tenancyResolver.GetUserId();
            var user = await _unitOfWork.ApplicationUsers.GetAsync(x => x.Id == userId);
            List<string> recipients = new List<string>(1);
            recipients.Add(user.Email);

            var sop = await _unitOfWork.Sops.GetAsync(x => x.Id == model.SopId);

            // Create model for handlebars email template
            var emailTemplateModel = new
            {
                Username = user.Forename,
                Title = model.Title,
                Reference = sop.Reference,
                Description = model.Description,
                Version = model.Version,
                Status = model.Status.ToString(),
                DateGenerated = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") ?? "N/A",
            };

            // Generate html email body content using the handlebars template
            string emailBody = await _templateService.RenderTemplateAsync("PdfExport", emailTemplateModel);
            string pdfName = "SOP" + "-" + sop.Reference + "-V" + model.Version + "-" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm");

            // Send the email with attachment
            _ = _emailService.SendEmailWithPdfAttachmentAsync(recipients, null, "Exported sop is ready", emailBody, template, pdfName);

            return template;
        }

        /// <summary>
        /// Creates a specific pdf template (template 1)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="customLogoUrl"></param>
        /// <returns></returns>
        private byte[] Template1(SopVersionDto model, string customLogoUrl)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    // Header
                    page.Header().Element(container => ComposeHeader(container, model, customLogoUrl));

                    // Content
                    page.Content().Element(container => ComposeContent(container, model).GetAwaiter().GetResult());

                    // Footer
                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();
        }

        /// <summary>
        /// Generates the content for the header
        /// </summary>
        /// <param name="container"></param>
        /// <param name="model"></param>
        /// <param name="customLogoUrl"></param>
        private void ComposeHeader(IContainer container, SopVersionDto model, string customLogoUrl)
        {
            container.Column(column =>
            {
                // Render logo if available
                if (!string.IsNullOrWhiteSpace(customLogoUrl))
                {
                    try
                    {
                        var imageBytes = DownloadImageAsync(customLogoUrl).GetAwaiter().GetResult();
                        if (imageBytes != null && imageBytes.Length > 0)
                        {
                            column.Item().Height(40).Image(imageBytes)
                                .WithCompressionQuality(ImageCompressionQuality.Medium);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading image: {ex.Message}");
                    }
                }

                // Render row below the logo
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text(model.Title)
                            .FontSize(24)
                            .Bold();

                        column.Item().Text($"SOP Version: {model.Version}")
                            .FontSize(12)
                            .FontColor(Colors.Grey.Medium);
                    });

                    row.ConstantItem(100).Column(column =>
                    {
                        column.Item().Text(model.Status.ToString())
                            .FontSize(12)
                            .Bold()
                            .FontColor(GetStatusColor(model.Status));
                    });
                });
            });
        }
        private IContainer CellStyle(IContainer container)
        {
            return container.Padding(5).BorderBottom(0);
        }

        /// <summary>
        /// Generates the main content for the pdf template (sop details and steps)
        /// </summary>
        /// <param name="container"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task ComposeContent(IContainer container, SopVersionDto model)
        {
            Dictionary<int, string> PpeDict = await _unitOfWork.Ppe.GetAll().ToDictionaryAsync(x => x.Id, x => x.Name);

            container.Column(column =>
            {
                // Metadata section
                column.Item().Border(0).Padding(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Cell().Element(CellStyle).Text("Author").FontSize(10).FontColor(Colors.Grey.Medium);
                    table.Cell().Element(CellStyle).Text(model.Author ?? "N/A").FontSize(12).Bold();

                    table.Cell().Element(CellStyle).Text("Approved By").FontSize(10).FontColor(Colors.Grey.Medium);
                    table.Cell().Element(CellStyle).Text(model.ApprovedBy ?? "N/A").FontSize(12).Bold();

                    table.Cell().Element(CellStyle).Text("Creation Date").FontSize(10).FontColor(Colors.Grey.Medium);
                    table.Cell().Element(CellStyle).Text(model.CreateDate?.ToShortDateString() ?? "N/A").FontSize(12).Bold();

                    table.Cell().Element(CellStyle).Text("Approval Date").FontSize(10).FontColor(Colors.Grey.Medium);
                    table.Cell().Element(CellStyle).Text(model.ApprovalDate?.ToShortDateString() ?? "N/A").FontSize(12).Bold();
                });

                // Description section
                column.Item().PaddingVertical(10).Column(desc =>
                {
                    desc.Item().Text("Description").FontSize(16).Bold();
                    desc.Item().PaddingTop(5).Text(model.Description);
                });

                // Hazards section
                if (model.SopHazards?.Any() == true)
                {
                    column.Item().PaddingVertical(10).Column(hazards =>
                    {
                        hazards.Item().Text("Hazards").FontSize(16).Bold();

                        foreach (var hazard in model.SopHazards)
                        {
                            hazards.Item().PaddingTop(5)
                                .Border(1)
                                .BorderColor(Colors.Red.Medium)
                                .Background(Colors.Red.Lighten5)
                                .Padding(10)
                                .Column(c =>
                                {
                                    c.Item().Text(hazard.Name).Bold().FontColor(Colors.Red.Darken4);
                                    c.Item().Text(hazard.ControlMeasure).FontColor(Colors.Red.Medium);
                                });
                        }
                    });
                }

                // Steps section
                if (model.SopSteps?.Any() == true)
                {
                    column.Item().PaddingVertical(10).Column(steps =>
                    {
                        steps.Item().Text("Procedure Steps").FontSize(16).Bold();

                        foreach (var (step, index) in model.SopSteps.Select((s, i) => (s, i)))
                        {
                            steps.Item().PaddingTop(5)
                                .Border(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .Background(Colors.Grey.Lighten4)
                                .Padding(10)
                                .Column(c =>
                                {
                                    // Step header with number
                                    c.Item().Text($"Step {index + 1}").Bold();

                                    // Image and content container
                                    c.Item().Row(row =>
                                    {
                                        // Image (if available)
                                        row.RelativeItem(2).Padding(5).Element(container =>
                                        {
                                            if (!string.IsNullOrEmpty(step.ImageUrl))
                                            {
                                                try
                                                {
                                                    // Download and load the image
                                                    var imageBytes = DownloadImageAsync(step.ImageUrl).GetAwaiter().GetResult();
                                                    if (imageBytes != null && imageBytes.Length > 0)
                                                    {
                                                        container
                                                            .Height(140)
                                                            .Image(imageBytes)
                                                            .FitArea()
                                                            .WithCompressionQuality(ImageCompressionQuality.Medium);
                                                    }
                                                    else
                                                    {
                                                        ComposeImagePlaceholder(container);
                                                    }
                                                }
                                                catch
                                                {
                                                    ComposeImagePlaceholder(container);
                                                }
                                            }
                                            else
                                            {
                                                ComposeImagePlaceholder(container);
                                            }
                                        });

                                        // Step content
                                        row.RelativeItem(3).Padding(5).Column(content =>
                                        {
                                            content.Item().Text(step.Title).Bold();
                                            content.Item().Text(step.Text);

                                            // PPE section 
                                            if (step.PpeIds?.Any() == true)
                                            {
                                                content.Item().PaddingTop(5).Column(ppe =>
                                                {
                                                    ppe.Item().Text("Required PPE:").Bold();
                                                    ppe.Item().PaddingTop(2).Row(ppeRow =>
                                                    {
                                                        foreach (var ppeId in step.PpeIds)
                                                        {
                                                            ppeRow.AutoItem().Background(Colors.Yellow.Lighten5)
                                                                .Border(1)
                                                                .BorderColor(Colors.Yellow.Medium)
                                                                .Padding(5)
                                                                .Text($"{PpeDict[ppeId]}")
                                                                .FontSize(10);
                                                        }
                                                    });
                                                });
                                            }
                                        });
                                    });
                                });
                        }
                    });
                }
            });
        }

        private void CreateMetadataItem(GridDescriptor grid, string label, string value)
        {
            grid.Item().Column(column =>
            {
                column.Item().Text(label).FontSize(10).FontColor(Colors.Grey.Medium);
                column.Item().Text(value).FontSize(12).Bold();
            });
        }

        /// <summary>
        /// Generates a placeholder for an image
        /// </summary>
        /// <param name="container"></param>
        private void ComposeImagePlaceholder(IContainer container)
        {
            container.Border(1)
                .BorderColor(Colors.Grey.Medium)
                .Background(Colors.Grey.Lighten3)
                .Height(140)
                .AlignCenter()
                .AlignMiddle()
                .Text("No image available")
                .FontColor(Colors.Grey.Medium);
        }

        /// <summary>
        /// Generates the content for the footer
        /// </summary>
        /// <param name="container"></param>
        private void ComposeFooter(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span("Generated on: ").FontSize(10);
                    text.Span(DateTime.Now.ToString("g")).FontSize(10);
                });

                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Page ").FontSize(10);
                    text.CurrentPageNumber().FontSize(10);
                    text.Span(" of ").FontSize(10);
                    text.TotalPages().FontSize(10);
                });
            });
        }

        /// <summary>
        /// Helper method for retrieving sop status colours
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private Color GetStatusColor(SopStatus status)
        {
            return status switch
            {
                SopStatus.Approved => Colors.Green.Medium,
                SopStatus.InReview => Colors.Orange.Medium,
                SopStatus.Draft => Colors.Grey.Medium,
                SopStatus.Archived => Colors.Grey.Darken4,
                SopStatus.Rejected => Colors.Red.Medium,
                _ => Colors.Grey.Medium
            };
        }

        /// <summary>
        /// Downloads an image
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        public async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetByteArrayAsync(imageUrl);
            }
        }
    }
}