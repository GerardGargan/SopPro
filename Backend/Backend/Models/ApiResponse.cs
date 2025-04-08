using System.Net;

namespace Backend.Models
{
    /// <summary>
    /// ApiResponse object, for returning a structured response from API requests
    /// </summary>
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Result { get; set; }
    }

}
