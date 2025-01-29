using Backend.Models.Dto;

namespace Backend.Service.Interface
{
    public interface IPdfService
    {
        public byte[] GeneratePdf(string templateName, SopVersionDto model);
    }
}