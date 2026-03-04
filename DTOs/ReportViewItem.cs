using GtaAdminReportsApp.Enums;

namespace GtaAdminReportsApp.DTOs;

public class ReportViewItem
{
    public int Id { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public string ViolationType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Priority { get; set; }
    public AdminClass TargetAdminClass { get; set; } = AdminClass.NotAssigned;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
