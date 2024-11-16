using Backend.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{

    [GeneralExceptionFilter]
    [Produces("application/json")]
    public class BaseController : ControllerBase
    {
    }

}
