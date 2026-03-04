using GtaAdminReportsApp.Models;
using GtaAdminReportsApp.Services;

namespace GtaAdminReportsApp.Data;

public static class DatabaseInitializer
{
    private static readonly string[] ViolationTypes =
    [
        "chat", "flood", "spam", "insult",
        "cheat", "hack", "aimbot", "wallhack",
        "ban_evasion", "ddos", "exploit", "dupe"
    ];

    public static async Task ResetAndSeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureDeletedAsync(cancellationToken);
        await EnsureCreatedAndSeedAsync(dbContext, cancellationToken);
    }

    public static async Task EnsureCreatedAndSeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var classifier = new ReportClassifierService();
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (dbContext.Users.Any() || dbContext.Reports.Any())
        {
            return;
        }

        var users = new List<User>
        {
            new() { Username = "AdminJunior_1", IsAdmin = true, AdminClass = Enums.AdminClass.Junior },
            new() { Username = "AdminSenior_1", IsAdmin = true, AdminClass = Enums.AdminClass.Senior },
            new() { Username = "AdminHead_1", IsAdmin = true, AdminClass = Enums.AdminClass.Head },
            new() { Username = "Player_One", IsAdmin = false },
            new() { Username = "Player_Two", IsAdmin = false }
        };

        await dbContext.Users.AddRangeAsync(users, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var reports = new List<Report>
        {
            new()
            {
                UserId = users[3].Id,
                PlayerName = users[3].Username,
                ViolationType = "flood",
                Description = "Многократные однотипные сообщения в чат.",
                Priority = 3,
                Status = "Open",
                CreatedAt = DateTime.UtcNow.AddMinutes(-30)
            },
            new()
            {
                UserId = users[4].Id,
                PlayerName = users[4].Username,
                ViolationType = "cheat",
                Description = "Подозрение на использование стороннего ПО.",
                Priority = 7,
                Status = "Open",
                CreatedAt = DateTime.UtcNow.AddMinutes(-20)
            },
            new()
            {
                UserId = users[3].Id,
                PlayerName = users[3].Username,
                ViolationType = "exploit",
                Description = "Обнаружено использование критического эксплойта.",
                Priority = 9,
                Status = "Open",
                CreatedAt = DateTime.UtcNow.AddMinutes(-10)
            }
        };

        foreach (var report in reports)
        {
            report.TargetAdminClass = classifier.DetectAdminClass(report);
        }

        await dbContext.Reports.AddRangeAsync(reports, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public static async Task AddBulkTestReportsAsync(
        AppDbContext dbContext,
        int count,
        CancellationToken cancellationToken = default)
    {
        var classifier = new ReportClassifierService();

        if (count <= 0)
        {
            return;
        }

        await EnsureCreatedAndSeedAsync(dbContext, cancellationToken);

        var players = dbContext.Users
            .Where(x => !x.IsAdmin)
            .ToList();

        if (players.Count == 0)
        {
            return;
        }

        var random = new Random();
        var reports = new List<Report>(capacity: count);

        for (var i = 0; i < count; i++)
        {
            var player = players[random.Next(players.Count)];
            var violationType = ViolationTypes[random.Next(ViolationTypes.Length)];
            var priority = random.Next(1, 11);

            reports.Add(new Report
            {
                UserId = player.Id,
                PlayerName = player.Username,
                ViolationType = violationType,
                Description = $"Тестовый репорт #{i + 1}: обнаружено нарушение типа {violationType}.",
                Priority = priority,
                Status = "Open",
                CreatedAt = DateTime.UtcNow.AddMinutes(-random.Next(1, 180))
            });
        }

        foreach (var report in reports)
        {
            report.TargetAdminClass = classifier.DetectAdminClass(report);
        }

        await dbContext.Reports.AddRangeAsync(reports, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
