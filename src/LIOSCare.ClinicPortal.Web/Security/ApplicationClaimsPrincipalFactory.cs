using LIOSCare.ClinicPortal.Web.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace LIOSCare.ClinicPortal.Web.Security;

public sealed class ApplicationClaimsPrincipalFactory(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IOptions<IdentityOptions> options)
    : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>(userManager, roleManager, options)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        if (!string.IsNullOrWhiteSpace(user.FullName)) identity.AddClaim(new Claim(PortalClaims.FullName, user.FullName));
        if (user.ClinicHospitalId.HasValue) identity.AddClaim(new Claim(PortalClaims.ClinicId, user.ClinicHospitalId.Value.ToString()));
        return identity;
    }
}
