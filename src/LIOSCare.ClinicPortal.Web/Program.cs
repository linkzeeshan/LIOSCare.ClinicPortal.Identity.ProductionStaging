using LIOSCare.ClinicPortal.Web.Data;
using LIOSCare.ClinicPortal.Web.Data.Entities;
using LIOSCare.ClinicPortal.Web.Security;
using LIOSCare.ClinicPortal.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
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

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute("dashboard", "dashboard", new { controller = "Dashboard", action = "Index" });
app.MapControllerRoute("default", "{controller=Account}/{action=Login}/{id?}");
app.Run();
