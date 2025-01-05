using System.Net;
using Backend.Data;
using Backend.Models;
using Backend.Models.Dto;
using Backend.Repository.Interface;
using Backend.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service.Implementation
{
    public class PpeService : IPpeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PpeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<List<PpeDto>>> GetAll()
        {
            List<PpeDto> ppeFromDb = await _unitOfWork.Ppe.GetAll().Select(x => PpeDto.FromPpe(x)).ToListAsync();
            return new ApiResponse<List<PpeDto>>
            {
                StatusCode = HttpStatusCode.OK,
                Result = ppeFromDb,
            };
        }
    }
}