using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Backend.Models;

namespace Backend.Filters
{
    public class GeneralExceptionFilter : ExceptionFilterAttribute
    {
        private static readonly Dictionary<Type, HttpStatusCode> _exceptionStatusCodes = new Dictionary<Type, HttpStatusCode>(8)
        {
            { typeof(ArgumentException), HttpStatusCode.BadRequest }, // 400
            { typeof(UnauthorizedAccessException), HttpStatusCode.Unauthorized }, // 401
            { typeof(KeyNotFoundException), HttpStatusCode.NotFound }, // 404
            { typeof(NotImplementedException), HttpStatusCode.MethodNotAllowed }, // 405
        };

        public GeneralExceptionFilter()
        {
        }

        public override void OnException(ExceptionContext context)
        {
            SetExceptionDetails(context);
        }

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            SetExceptionDetails(context);
        }

        private static void SetExceptionDetails(ExceptionContext context)
        {
            var apiResponse = new ApiResponse();

            // Check if the exception is mapped to a specific status code
            var isExceptionMapped = _exceptionStatusCodes.TryGetValue(context.Exception.GetType(), out HttpStatusCode exceptionStatusCode);

            if (!isExceptionMapped)
            {
                // Handle unmapped exceptions with a 500 Internal Server Error
                apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                apiResponse.IsSuccess = false;
                apiResponse.ErrorMessages.Add(context.Exception.Message);

                context.Result = new JsonResult(apiResponse) // Use JsonResult for structured JSON responses
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };

                context.ExceptionHandled = true;
                return;
            }

            // Handle mapped exceptions
            apiResponse.StatusCode = exceptionStatusCode;
            apiResponse.IsSuccess = false;
            apiResponse.ErrorMessages.Add(context.Exception.Message);

            context.Result = new JsonResult(apiResponse)
            {
                StatusCode = (int)exceptionStatusCode
            };

            context.ExceptionHandled = true;
        }

    }

}
