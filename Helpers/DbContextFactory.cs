using GtaAdminReportsApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

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
        var provider = (configuration["DatabaseProvider"] ?? "Postgres").Trim().ToLowerInvariant();

        if (provider is "sqlite")
        {
            var sqliteConnection = configuration.GetConnectionString("SqliteDatabase")
                ?? throw new InvalidOperationException("Connection string 'SqliteDatabase' is not configured.");

            EnsureSqliteDirectory(sqliteConnection);
            builder.UseSqlite(sqliteConnection);
        }
        else
        {
            var postgresConnection = configuration.GetConnectionString("PostgresDatabase")
                ?? throw new InvalidOperationException("Connection string 'PostgresDatabase' is not configured.");
            builder.UseNpgsql(postgresConnection);
        }

        return new AppDbContext(builder.Options);
    }

    private static void EnsureSqliteDirectory(string sqliteConnection)
    {
        const string prefix = "Data Source=";

        if (!sqliteConnection.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var dbPath = sqliteConnection[prefix.Length..].Trim().Trim('"');
        if (string.IsNullOrWhiteSpace(dbPath))
        {
            return;
        }

        var fullPath = Path.IsPathRooted(dbPath)
            ? dbPath
            : Path.Combine(AppContext.BaseDirectory, dbPath);

        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
