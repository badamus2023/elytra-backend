namespace Drones.src.Api.Common.DTOs
{
    public class ApiErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public string? Code { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
