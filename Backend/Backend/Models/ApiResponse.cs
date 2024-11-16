using System.Net;

namespace Backend.Models
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Result { get; set; }
    }

}
