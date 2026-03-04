using GtaAdminReportsApp.Data;
using GtaAdminReportsApp.DTOs;
using GtaAdminReportsApp.Enums;
using Microsoft.EntityFrameworkCore;

namespace GtaAdminReportsApp.Services;

public class ReportService(AppDbContext dbContext, ReportClassifierService classifierService)
{
    public async Task<List<ReportViewItem>> GetOpenReportsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Reports
            .AsNoTracking()
            .Where(x => x.Status == "Open")
            .OrderByDescending(x => x.Priority)
            .ThenBy(x => x.CreatedAt)
            .Select(x => new ReportViewItem
            {
                Id = x.Id,
                PlayerName = x.PlayerName,
                ViolationType = x.ViolationType,
                Description = x.Description,
                Priority = x.Priority,
                TargetAdminClass = x.TargetAdminClass,
                Status = x.Status,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<int> SortOpenReportsAsync(CancellationToken cancellationToken = default)
    {
        var openReports = await dbContext.Reports
            .Where(x => x.Status == "Open")
            .ToListAsync(cancellationToken);

        foreach (var report in openReports)
        {
            report.TargetAdminClass = classifierService.DetectAdminClass(report);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return openReports.Count;
    }

    public static List<ReportViewItem> ApplyAdminClassFilter(
        IEnumerable<ReportViewItem> reports,
        string selectedFilter)
    {
        if (selectedFilter == "Все")
        {
            return reports.ToList();
        }

        if (!Enum.TryParse<AdminClass>(selectedFilter, out var targetClass))
        {
            return reports.ToList();
        }

        return reports
            .Where(x => x.TargetAdminClass == targetClass)
            .ToList();
    }
}
