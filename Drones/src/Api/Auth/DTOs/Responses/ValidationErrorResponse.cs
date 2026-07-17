namespace Drones.src.Api.Auth.DTOs.Responses
{
    public class ValidationErrorResponse
    {
        public string Message { get; set; } = "Validation Failed";
        public List<ValidationError> Errors { get; set; } = [];
    }

    public class ValidationError
    {
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
