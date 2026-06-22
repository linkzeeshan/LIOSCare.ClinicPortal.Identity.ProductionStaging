using LIOSCare.ClinicPortal.Web.Data;
using LIOSCare.ClinicPortal.Web.Data.Entities;
using LIOSCare.ClinicPortal.Web.Security;
using LIOSCare.ClinicPortal.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

if (builder.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/home/error");
    app.UseHsts();
}

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
