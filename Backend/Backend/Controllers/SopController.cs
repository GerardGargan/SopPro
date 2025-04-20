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

        /// <summary>
        /// Creates a SOP
        /// </summary>
        /// <param name="sopDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateSop([FromBody] SopDto sopDto)
        {
            var apiResponse = await _sopService.CreateSop(sopDto);
            return Created(string.Empty, apiResponse);
        }

        /// <summary>
        /// Gets all SOPs
        /// </summary>
        /// <param name="search"></param>
        /// <param name="status"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="isFavourite"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApiResponse<List<SopDto>>))]
        public async Task<IActionResult> GetAll([FromQuery] string search, [FromQuery] int? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] bool isFavourite = false, [FromQuery] string sortBy = "recent", [FromQuery] string sortOrder = "desc")
        {
            var apiResponse = await _sopService.GetAllSops(search, status, page, pageSize, isFavourite, sortBy, sortOrder);
            return Ok(apiResponse);
        }

        /// <summary>
        /// Retrieves the latest version of a SOP by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApiResponse<SopDto>))]
        [Route("{id:int}")]
        public async Task<IActionResult> GetSopLatestVersion(int id)
        {
            var apiResponse = await _sopService.GetLatestSopVersion(id);
            return Ok(apiResponse);
        }

        /// <summary>
        /// Updates a SOP. A new SOP version will be created if the SOP is approved. Otherwise the existing SOP version will be updated.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sopDto"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(200, Type = typeof(ApiResponse<SopDto>))]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateSop(int id, [FromBody] SopDto sopDto)
        {
            var apiResponse = await _sopService.UpdateSop(id, sopDto);
            return Ok(apiResponse);
        }

        /// <summary>
        /// Deletes a range of SOP's, and associated versions, steps, hazards and images in blob storage.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        [Route("delete")]
        public async Task<IActionResult> DeleteSops([FromBody] List<int> ids)
        {
            var apiResponse = await _sopService.DeleteSops(ids);
            return Ok(apiResponse);
        }

        /// <summary>
        /// Deletes a SOP by its id, and all associates versions, hazards, steps and images in blob storage.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteSop(int id)
        {
            List<int> ids = new List<int>(1) {
                id
            };

            var apiResponse = await _sopService.DeleteSops(ids);
            return Ok(apiResponse);
        }

        /// <summary>
        /// Uploads a file to blob storage
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        [Route("upload")]
        public async Task<IActionResult> UploadImage([FromForm] FileDto file)
        {
            var apiResponse = await _sopService.UploadImage(file);
            return Ok(apiResponse);
        }

        /// <summary>
        /// Adds a SOP to the users favourites
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        [Route("{id:int}/favourite")]
        public async Task<IActionResult> FavouriteSop(int id)
        {
            var apiResponse = await _sopService.AddToFavourites(id);
            return Ok(apiResponse);
        }

        /// <summary>
        /// Removes a SOP from a users favourites
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        [Route("{id:int}/favourite")]
        public async Task<IActionResult> UnfavouriteSop(int id)
        {
            var apiResponse = await _sopService.RemoveFromFavourites(id);
            return Ok(apiResponse);
        }

        /// <summary>
        /// Updates an SOPs status to Approved (Admin only).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = StaticDetails.Role_Admin)]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        [Route("{id:int}/approve")]
        public async Task<IActionResult> ApproveSop(int id)
        {
            var apiResponse = await _sopService.ApproveSop(id);
            return Ok(apiResponse);
        }

        /// <summary>
        /// Updates an SOPs status from In Review to Rejected
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = StaticDetails.Role_Admin)]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        [Route("{id:int}/reject")]
        public async Task<IActionResult> RejectSop(int id)
        {
            var apiResponse = await _sopService.RejectSop(id);
            return Ok(apiResponse);
        }

        /// <summary>
        /// Updates a SOPs status to In Review and notifies administrators
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        [Route("{id:int}/requestapproval")]
        public async Task<IActionResult> RequestApproval(int id)
        {
            var apiResponse = await _sopService.RequestApproval(id);
            return Ok(apiResponse);
        }

        /// <summary>
        /// Generates a PDF of a SOP version
        /// </summary>
        /// <param name="sopVersionId"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(FileContentResult))]
        [Route("{sopVersionId:int}/pdf")]
        public async Task<IActionResult> GeneratePdf(int sopVersionId)
        {
            var sopServiceResponse = await _sopService.GetSopVersion(sopVersionId);
            SopVersionDto sopVersion = sopServiceResponse;
            var pdf = await _pdfService.GeneratePdf("template1", sopVersion);

            return File(pdf, "application/pdf", "test.pdf");

        }

        /// <summary>
        /// Generates a SOP via Artificatal Intelligence
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(SopDto))]
        [Route("aigenerator")]
        public async Task<IActionResult> GenerateAiSop([FromBody] AiRequestDto model)
        {
            var result = await _sopService.GenerateAiSop(model);
            return Created(string.Empty, result);
        }

        /// <summary>
        /// Provides analytics on SOPs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApiResponse<AnalyticsResponseDto>))]
        [Route("analytics")]
        public async Task<IActionResult> GetAnalytics()
        {
            var analyticsDto = await _sopService.GetAnalytics();
            return Ok(analyticsDto);
        }

        /// <summary>
        /// Reverts a SOP to a previous version. All latter versions are deleted permenantly.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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