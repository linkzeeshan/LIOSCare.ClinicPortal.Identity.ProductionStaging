namespace LIOSCare.ClinicPortal.Web.Security;

public static class PortalRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string Doctor = "Doctor";
}

public static class PortalClaims
{
    public const string Permission = "permission";
    public const string ClinicId = "clinic_id";
    public const string FullName = "full_name";
}

public static class PortalPermissions
{
    public const string ManageSuperAdmins = "manage:super-admins";
    public const string ManageFacilities = "manage:facilities";
    public const string ManageClinicUsers = "manage:clinic-users";
    public const string ManageDoctors = "manage:doctors";
    public const string ManagePackages = "manage:packages";
    public const string DoctorWorkspace = "doctor:workspace";
    public const string ViewNotifications = "view:notifications";
}

public static class PortalPolicies
{
    public const string SuperAdminOnly = "SuperAdminOnly";
    public const string ManageSuperAdmins = "ManageSuperAdmins";
    public const string ManageFacilities = "ManageFacilities";
    public const string ManageClinicUsers = "ManageClinicUsers";
    public const string ManageDoctors = "ManageDoctors";
    public const string ManagePackages = "ManagePackages";
    public const string DoctorWorkspace = "DoctorWorkspace";
}
