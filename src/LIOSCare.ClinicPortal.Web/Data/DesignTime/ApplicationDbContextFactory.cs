using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LIOSCare.ClinicPortal.Web.Data.DesignTime;

public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("LIOSCARE_CONNECTION")
                 ?? "Host=localhost;Port=5432;Database=social_platform_db;Username=postgres;Password=postgres;Include Error Detail=true";
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseNpgsql(cs, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "portal"));
        return new ApplicationDbContext(builder.Options);
    }
}
