namespace Drones.src.Api.Common.DTOs
{
    public class NotificationMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Category { get; set; } = "system";
        public string Severity { get; set; } = "info";
        public string Presentation { get; set; } = "toast";
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
        public Guid? OrderId { get; set; }
        public string? Status { get; set; }
    }
}
