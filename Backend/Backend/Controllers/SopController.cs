using Backend.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers {
    [Route("api/sop")]
    [ApiController]
    public class SopController : BaseApiController {
        private readonly ISopService _sopService;

        public SopController(ISopService sopService) {
            _sopService = sopService;
        }
    }
}