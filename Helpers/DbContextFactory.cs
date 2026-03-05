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
        var postgresConnection = configuration.GetConnectionString("PostgresDatabase")
            ?? throw new InvalidOperationException("Connection string 'PostgresDatabase' is not configured.");
        builder.UseNpgsql(postgresConnection);

        return new AppDbContext(builder.Options);
    }
}
