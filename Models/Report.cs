using GtaAdminReportsApp.Enums;

namespace GtaAdminReportsApp.Models;

public class Report
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string ViolationType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Priority { get; set; }
    public string Status { get; set; } = "Open";
    public AdminClass TargetAdminClass { get; set; } = AdminClass.NotAssigned;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
}
