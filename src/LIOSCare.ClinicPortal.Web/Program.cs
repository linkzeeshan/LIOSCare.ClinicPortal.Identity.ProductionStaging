using LIOSCare.ClinicPortal.Web.Data;
using LIOSCare.ClinicPortal.Web.Data.Entities;
using LIOSCare.ClinicPortal.Web.Exceptions;
using LIOSCare.ClinicPortal.Web.Security;
using LIOSCare.ClinicPortal.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using AspNetCoreRateLimit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection is missing.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "portal");
    });
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationClaimsPrincipalFactory>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".LIOSCare.ClinicPortal.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.AccessDeniedPath = "/account/access-denied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PortalPolicies.SuperAdminOnly, p => p.RequireRole(PortalRoles.SuperAdmin));
    options.AddPolicy(PortalPolicies.ManageFacilities, p => p.RequireClaim(PortalClaims.Permission, PortalPermissions.ManageFacilities));
    options.AddPolicy(PortalPolicies.ManageSuperAdmins, p => p.RequireClaim(PortalClaims.Permission, PortalPermissions.ManageSuperAdmins));
    options.AddPolicy(PortalPolicies.ManageClinicUsers, p => p.RequireClaim(PortalClaims.Permission, PortalPermissions.ManageClinicUsers));
    options.AddPolicy(PortalPolicies.ManageDoctors, p => p.RequireClaim(PortalClaims.Permission, PortalPermissions.ManageDoctors));
    options.AddPolicy(PortalPolicies.ManagePackages, p => p.RequireClaim(PortalClaims.Permission, PortalPermissions.ManagePackages));
    options.AddPolicy(PortalPolicies.DoctorWorkspace, p => p.RequireClaim(PortalClaims.Permission, PortalPermissions.DoctorWorkspace));
});

builder.Services.Configure<PortalRulesOptions>(builder.Configuration.GetSection("PortalRules"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IIdentityManagementService, IdentityManagementService>();
builder.Services.AddScoped<IFacilityService, FacilityService>();
builder.Services.AddScoped<IClinicAdminService, ClinicAdminService>();
builder.Services.AddScoped<IDoctorWorkspaceService, DoctorWorkspaceService>();
builder.Services.AddHostedService<ChatSessionAutoCloseWorker>();

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>()
    .AddCheck("Application", () => HealthCheckResult.Healthy("Application is running"));

// Add Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup") || 
    builder.Configuration.GetValue<bool>("Portal:AutoMigrateOnStartup"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();
    await ApplyMigrationsAsync(db, logger);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/home/error");
    app.UseHsts();
}

// Global exception handler for doctor profile not found
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (DoctorProfileNotFoundException)
    {
        context.Response.Clear();
        context.Response.StatusCode = 403;
        context.Response.Redirect($"/home/error?message={Uri.EscapeDataString("Your doctor profile is not configured. Please contact your administrator to set up your profile.")}");
    }
});

// Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers["Content-Security-Policy"] = 
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
            "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdn.jsdelivr.net; " +
            "font-src 'self' https://fonts.gstatic.com; " +
            "img-src 'self' data:; " +
            "connect-src 'self'";
    }
    
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

// Rate Limiting
app.UseIpRateLimiting();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Health Checks
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                description = x.Value.Description,
                duration = x.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

// Default route - Account/Login page
app.MapControllerRoute("default", "{controller=Account}/{action=Login}/{id?}");

// Specific routes
app.MapControllerRoute("dashboard", "dashboard", new { controller = "Dashboard", action = "Index" });

try
{
    Log.Information("Starting LIOSCare Clinic Portal");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static async Task ApplyMigrationsAsync(ApplicationDbContext db, Microsoft.Extensions.Logging.ILogger logger)
{
    const int maxRetries = 3;
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            await db.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");
            return;
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "42701") // column already exists
        {
            logger.LogWarning(
                "Migration failed with '42701 column already exists' (attempt {Attempt}/{Max}). " +
                "This means portal tables were partially created before migrations ran. " +
                "Marking pending migrations as applied and retrying...", attempt, maxRetries);

            // The production DB has the schema but __EFMigrationsHistory is missing entries.
            // Find which migrations are pending and mark them as applied.
            await MarkPartiallyAppliedMigrationsAsync(db, logger);

            if (attempt == maxRetries)
            {
                logger.LogError(ex, "Migration failed after {Max} attempts.", maxRetries);
                throw;
            }
        }
    }
}

static async Task MarkPartiallyAppliedMigrationsAsync(ApplicationDbContext db, Microsoft.Extensions.Logging.ILogger logger)
{
    // Get all pending migrations
    var pending = (await db.Database.GetPendingMigrationsAsync()).ToList();
    if (pending.Count == 0) return;

    // Get EF product version from the assembly
    var efVersion = typeof(Microsoft.EntityFrameworkCore.DbContext).Assembly
        .GetName().Version?.ToString(3) ?? "9.0.0";

    var conn = db.Database.GetDbConnection();
    await conn.OpenAsync();
    try
    {
        // For each pending migration, check if the tables it would create already exist.
        // If so, mark it as applied in __EFMigrationsHistory.
        foreach (var migrationId in pending)
        {
            // Check whether the portal schema already exists with data
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT COUNT(*) FROM information_schema.tables
WHERE table_schema = 'portal' AND table_name IN 
    ('session_reports','chat_sessions','doctor_profiles','clinics_hospitals')";
            var count = (long)(await cmd.ExecuteScalarAsync() ?? 0L);

            if (count > 0)
            {
                using var insertCmd = conn.CreateCommand();
                insertCmd.CommandText = $@"
INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
VALUES ('{migrationId}', '{efVersion}')
ON CONFLICT (""MigrationId"") DO NOTHING;";
                await insertCmd.ExecuteNonQueryAsync();
                logger.LogWarning(
                    "Marked migration '{MigrationId}' as applied because portal schema already exists.",
                    migrationId);
            }
        }
    }
    finally
    {
        await conn.CloseAsync();
    }
}
