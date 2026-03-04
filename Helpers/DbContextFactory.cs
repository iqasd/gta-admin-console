using GtaAdminReportsApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GtaAdminReportsApp.Helpers;

public static class DbContextFactory
{
    public static AppDbContext Create()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        var provider = configuration["DatabaseProvider"]?.Trim().ToLowerInvariant() ?? "sqlite";

        if (provider == "mysql")
        {
            var mysqlConnection = configuration.GetConnectionString("FiveMDatabase")
                ?? throw new InvalidOperationException("Connection string 'FiveMDatabase' is not configured.");
            builder.UseMySql(mysqlConnection, ServerVersion.AutoDetect(mysqlConnection));
        }
        else
        {
            var sqliteConnection = configuration.GetConnectionString("SqliteDatabase")
                ?? throw new InvalidOperationException("Connection string 'SqliteDatabase' is not configured.");
            builder.UseSqlite(sqliteConnection);
        }

        return new AppDbContext(builder.Options);
    }
}
