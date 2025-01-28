using Backend.Models.DatabaseModels;
using Backend.Models.Dto;
using Backend.Service.Interface;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Backend.Service.Implementation
{
    public class PdfService : IPdfService
    {
        public byte[] GeneratePdf(string templateName, SopDto model)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            switch (templateName.ToLower())
            {
                case "template1":
                    return Template1(model);
                default:
                    throw new Exception("Invalid template name");
            }
        }

        private byte[] Template1(SopDto model)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Header().Text("Header goes here").FontSize(36).Bold();

                    page.Content().Column(column =>
                    {
                        column.Item().Text("Test");
                        column.Item().Text("Second batch of text");
                        column.Item().Image(Placeholders.Image(200, 100));
                    });

                    page.Footer().Text(text =>
                    {
                        text.AlignRight();
                        text.CurrentPageNumber();
                    });
                });
            }).GeneratePdf();
        }
    }
}