using GtaAdminReportsApp.Data;
using GtaAdminReportsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GtaAdminReportsApp.Services;

public class AppLoggerService(AppDbContext dbContext)
{
    public Task LogInfoAsync(string action, string message, string? details = null, CancellationToken cancellationToken = default)
        => LogAsync("Info", action, message, details, cancellationToken);

    public Task LogErrorAsync(string action, string message, string? details = null, CancellationToken cancellationToken = default)
        => LogAsync("Error", action, message, details, cancellationToken);

    public Task<List<AppLog>> GetLatestLogsAsync(int take = 200, CancellationToken cancellationToken = default)
    {
        return dbContext.AppLogs
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public Task<int> ClearLogsAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.AppLogs.ExecuteDeleteAsync(cancellationToken);
    }

    private async Task LogAsync(
        string level,
        string action,
        string message,
        string? details,
        CancellationToken cancellationToken)
    {
        dbContext.Add(new AppLog
        {
            Level = level,
            Action = action,
            Message = message,
            Details = details,
            CreatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
