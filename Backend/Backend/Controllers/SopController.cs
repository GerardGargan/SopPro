using Backend.Models;
using Microsoft.AspNetCore.Http;
using Backend.Models.Dto;
using Backend.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Utility;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Net;

namespace Backend.Controllers
{
    [Route("api/sop")]
    [ApiController]
    [Authorize]
    public class SopController : BaseApiController
    {
        private readonly ISopService _sopService;
        private readonly IPdfService _pdfService;
        private readonly IChatCompletionService _chatService;

        public SopController(ISopService sopService, IPdfService pdfService, IChatCompletionService chatService)
        {

            _sopService = sopService;
            _pdfService = pdfService;
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSop([FromBody] SopDto sopDto)
        {
            var apiResponse = await _sopService.CreateSop(sopDto);
            return Ok(apiResponse);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string search, [FromQuery] int? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] bool isFavourite = false, [FromQuery] string sortBy = "recent", [FromQuery] string sortOrder = "desc")
        {
            var apiResponse = await _sopService.GetAllSops(search, status, page, pageSize, isFavourite, sortBy, sortOrder);
            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetSopLatestVersion(int id)
        {
            var apiResponse = await _sopService.GetLatestSopVersion(id);
            return Ok(apiResponse);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateSop(int id, [FromBody] SopDto sopDto)
        {
            var apiResponse = await _sopService.UpdateSop(id, sopDto);
            return Ok(apiResponse);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<IActionResult> DeleteSops([FromBody] List<int> ids)
        {
            var apiResponse = await _sopService.DeleteSops(ids);
            return Ok(apiResponse);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteSop(int id)
        {
            List<int> ids = new List<int>(1) {
                id
            };

            var apiResponse = await _sopService.DeleteSops(ids);
            return Ok(apiResponse);
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadImage([FromForm] FileDto file)
        {
            var apiResponse = await _sopService.UploadImage(file);
            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("{id:int}/favourite")]
        public async Task<IActionResult> FavouriteSop(int id)
        {
            var apiResponse = await _sopService.AddToFavourites(id);
            return Ok(apiResponse);
        }

        [HttpDelete]
        [Route("{id:int}/favourite")]
        public async Task<IActionResult> UnfavouriteSop(int id)
        {
            var apiResponse = await _sopService.RemoveFromFavourites(id);
            return Ok(apiResponse);
        }

        [Authorize(Roles = StaticDetails.Role_Admin)]
        [HttpGet]
        [Route("{id:int}/approve")]
        public async Task<IActionResult> ApproveSop(int id)
        {
            var apiResponse = await _sopService.ApproveSop(id);
            return Ok(apiResponse);
        }

        [Authorize(Roles = StaticDetails.Role_Admin)]
        [HttpGet]
        [Route("{id:int}/reject")]
        public async Task<IActionResult> RejectSop(int id)
        {
            var apiResponse = await _sopService.RejectSop(id);
            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("{id:int}/requestapproval")]
        public async Task<IActionResult> RequestApproval(int id)
        {
            var apiResponse = await _sopService.RequestApproval(id);
            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("{sopVersionId:int}/pdf")]
        public async Task<IActionResult> GeneratePdf(int sopVersionId)
        {
            var sopServiceResponse = await _sopService.GetSopVersion(sopVersionId);
            SopVersionDto sopVersion = sopServiceResponse;
            var pdf = await _pdfService.GeneratePdf("template1", sopVersion);

            return File(pdf, "application/pdf", "test.pdf");

        }

        [HttpPost]
        [Route("aigenerator")]
        public async Task<IActionResult> GenerateAiSop([FromBody] AiRequestDto model)
        {
            var result = await _sopService.GenerateAiSop(model);
            return Ok(result);
        }

        [HttpGet]
        [Route("analytics")]
        public async Task<IActionResult> GetAnalytics()
        {
            var analyticsDto = await _sopService.GetAnalytics();
            return Ok(analyticsDto);
        }

        [HttpPost]
        [Route("revert")]
        [Authorize(Roles = StaticDetails.Role_Admin)]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> RevertSopVersion([FromBody] RevertRequestDto model)
        {

            await _sopService.RevertSop(model);
            ApiResponse response = new ApiResponse()
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                SuccessMessage = "Version reverted successfully"
            }
            ;

            return Ok(response);
        }

    }
}