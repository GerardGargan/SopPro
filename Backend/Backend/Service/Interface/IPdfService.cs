using Backend.Models.Dto;

namespace Backend.Service.Interface
{
    public interface IPdfService
    {
        public Task<byte[]> GeneratePdf(string templateName, SopVersionDto model);
    }
}