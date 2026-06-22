using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LIOSCare.ClinicPortal.Web.Data.DesignTime;

public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection")
                 ?? "Host=127.0.0.1;Port=5432;Database=lioscare_doctor_dashboard;Username=postgres;Password=Z@@@a123;Include Error Detail=true";
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseNpgsql(cs, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "portal"));
        return new ApplicationDbContext(builder.Options);
    }
}
