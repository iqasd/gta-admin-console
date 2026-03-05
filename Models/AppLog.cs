namespace GtaAdminReportsApp.Models;

public class AppLog
{
    public long Id { get; set; }
    public string Level { get; set; } = "Info";
    public string Action { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
