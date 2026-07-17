namespace Drones.src.Api.Auth.DTOs.Responses
{
    public class ErrorResponse
    {
        public string Message { get; set; } = "An error occurred";
        public string? Code { get; set; }
    }
}
