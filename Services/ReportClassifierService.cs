using GtaAdminReportsApp.Enums;
using GtaAdminReportsApp.Models;

namespace GtaAdminReportsApp.Services;

public class ReportClassifierService
{
    public AdminClass DetectAdminClass(Report report)
    {
        var violation = report.ViolationType.Trim().ToLowerInvariant();

        return violation switch
        {
            "chat" or "flood" or "spam" or "insult" => AdminClass.Junior,
            "cheat" or "hack" or "aimbot" or "wallhack" => AdminClass.Senior,
            "ban_evasion" or "ddos" or "exploit" or "dupe" => AdminClass.Head,
            _ => ResolveByPriority(report.Priority)
        };
    }

    private static AdminClass ResolveByPriority(int priority)
    {
        return priority switch
        {
            >= 8 => AdminClass.Head,
            >= 5 => AdminClass.Senior,
            _ => AdminClass.Junior
        };
    }
}
