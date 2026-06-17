using LIOSCare.ClinicPortal.Web.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIOSCare.ClinicPortal.Web.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("202606160011_SeedIdentityTenantPortal")]
public partial class SeedIdentityTenantPortal : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
        INSERT INTO "AspNetRoles"("Id", "Name", "NormalizedName", "ConcurrencyStamp", "Description") VALUES
        ('10000000-0000-0000-0000-000000000001','SuperAdmin','SUPERADMIN',gen_random_uuid()::text,'Full platform access'),
        ('10000000-0000-0000-0000-000000000002','Admin','ADMIN',gen_random_uuid()::text,'Clinic/Hospital administrator'),
        ('10000000-0000-0000-0000-000000000003','Doctor','DOCTOR',gen_random_uuid()::text,'Doctor workspace user')
        ON CONFLICT ("Id") DO NOTHING;

        INSERT INTO "AspNetRoleClaims"("RoleId", "ClaimType", "ClaimValue") VALUES
        ('10000000-0000-0000-0000-000000000001','permission','manage:super-admins'),
        ('10000000-0000-0000-0000-000000000001','permission','manage:facilities'),
        ('10000000-0000-0000-0000-000000000001','permission','manage:clinic-users'),
        ('10000000-0000-0000-0000-000000000001','permission','manage:doctors'),
        ('10000000-0000-0000-0000-000000000001','permission','manage:packages'),
        ('10000000-0000-0000-0000-000000000002','permission','manage:clinic-users'),
        ('10000000-0000-0000-0000-000000000002','permission','manage:doctors'),
        ('10000000-0000-0000-0000-000000000002','permission','manage:packages'),
        ('10000000-0000-0000-0000-000000000003','permission','doctor:workspace'),
        ('10000000-0000-0000-0000-000000000003','permission','view:notifications')
        ON CONFLICT DO NOTHING;

        INSERT INTO portal.clinics_hospitals("Id","Name","Type","Status","ContactEmail","ContactPhone","City","Country","Address","AdminUserId","CreatedAt","UpdatedAt") VALUES
        ('20000000-0000-0000-0000-000000000001','LIOS Care Demo Clinic','Clinic','Approved','clinicadmin@lioscare.local','+1-555-0100','New York','United States','Demo clinic address', '30000000-0000-0000-0000-000000000002', now(), now())
        ON CONFLICT ("Id") DO UPDATE SET "Name"=excluded."Name", "Status"='Approved', "AdminUserId"=excluded."AdminUserId", "UpdatedAt"=now();

        INSERT INTO "AspNetUsers"("Id","UserName","NormalizedUserName","Email","NormalizedEmail","EmailConfirmed","PasswordHash","SecurityStamp","ConcurrencyStamp","PhoneNumberConfirmed","TwoFactorEnabled","LockoutEnabled","AccessFailedCount","FullName","Status","ClinicHospitalId","CreatedAt","UpdatedAt") VALUES
        ('30000000-0000-0000-0000-000000000001','superadmin@lioscare.local','SUPERADMIN@LIOSCARE.LOCAL','superadmin@lioscare.local','SUPERADMIN@LIOSCARE.LOCAL',true,'',gen_random_uuid()::text,gen_random_uuid()::text,false,false,true,0,'System Super Admin','Active',NULL,now(),now()),
        ('30000000-0000-0000-0000-000000000002','clinicadmin@lioscare.local','CLINICADMIN@LIOSCARE.LOCAL','clinicadmin@lioscare.local','CLINICADMIN@LIOSCARE.LOCAL',true,'',gen_random_uuid()::text,gen_random_uuid()::text,false,false,true,0,'Demo Clinic Admin','Active','20000000-0000-0000-0000-000000000001',now(),now()),
        ('30000000-0000-0000-0000-000000000003','doctor@lioscare.local','DOCTOR@LIOSCARE.LOCAL','doctor@lioscare.local','DOCTOR@LIOSCARE.LOCAL',true,'',gen_random_uuid()::text,gen_random_uuid()::text,false,false,true,0,'Dr. Julia Adams','Active','20000000-0000-0000-0000-000000000001',now(),now())
        ON CONFLICT ("Id") DO UPDATE SET "PasswordHash"=excluded."PasswordHash", "Status"='Active', "UpdatedAt"=now();

        INSERT INTO "AspNetUserRoles"("UserId","RoleId") VALUES
        ('30000000-0000-0000-0000-000000000001','10000000-0000-0000-0000-000000000001'),
        ('30000000-0000-0000-0000-000000000002','10000000-0000-0000-0000-000000000002'),
        ('30000000-0000-0000-0000-000000000003','10000000-0000-0000-0000-000000000003')
        ON CONFLICT DO NOTHING;

        INSERT INTO "AspNetUserClaims"("UserId","ClaimType","ClaimValue") VALUES
        ('30000000-0000-0000-0000-000000000002','clinic_id','20000000-0000-0000-0000-000000000001'),
        ('30000000-0000-0000-0000-000000000003','clinic_id','20000000-0000-0000-0000-000000000001')
        ON CONFLICT DO NOTHING;

        INSERT INTO portal.service_tiers("Id","Name","DurationMinutes","Price","Currency","ServiceType","IsActive","SortOrder") VALUES
        ('40000000-0000-0000-0000-000000000001','Quick Chat 30',30,25,'USD','Chat',true,1),
        ('40000000-0000-0000-0000-000000000002','Quick Chat 40',40,35,'USD','Chat',true,2),
        ('40000000-0000-0000-0000-000000000003','Online Consultation 40',40,99,'USD','OnlineConsultation',true,3)
        ON CONFLICT ("Id") DO UPDATE SET "Price"=excluded."Price", "DurationMinutes"=excluded."DurationMinutes", "ServiceType"=excluded."ServiceType";

        INSERT INTO portal.doctor_profiles("Id","UserId","ClinicId","DisplayName","Specializations","YearsExperience","LicenseNumber","Bio","IsAvailable","MaxConcurrentSessions","Status","ChatSessionPrice","OnlineConsultationPrice","ChatSessionMinutes","OnlineConsultationMinutes","Currency","WorkingDays","WorkStartTime","WorkEndTime","CreatedAt","UpdatedAt") VALUES
        ('50000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000003','20000000-0000-0000-0000-000000000001','Dr. Julia Adams',ARRAY['Depression','Anxiety','Trauma'],10,'LIC-APYS-28491','Licensed clinical psychologist focused on anxiety, trauma and practical therapy.',true,3,'Active',25,99,30,40,'USD',ARRAY['Monday','Tuesday','Wednesday','Thursday','Friday'],'09:00','17:00',now(),now())
        ON CONFLICT ("Id") DO UPDATE SET "DisplayName"=excluded."DisplayName", "Status"='Active', "UpdatedAt"=now();

        INSERT INTO portal.patient_accounts("Id","AnonymousCode","AnonymousDisplayName","UserPortalEmail","CreatedAt") VALUES
        ('60000000-0000-0000-0000-000000000001','ANON-8F3K','Anonymous Patient A',NULL,now()),
        ('60000000-0000-0000-0000-000000000002','ANON-2Q9L','Anonymous Patient B',NULL,now())
        ON CONFLICT ("Id") DO NOTHING;

        INSERT INTO portal.chat_session_jobs("Id","PatientId","PreferredDoctorId","ClinicId","ServiceTierId","JobType","Status","PatientNotePublic","OfferExpiresAt","CreatedAt","UpdatedAt") VALUES
        ('70000000-0000-0000-0000-000000000001','60000000-0000-0000-0000-000000000001',NULL,'20000000-0000-0000-0000-000000000001','40000000-0000-0000-0000-000000000001','QuickChat','Open','Patient needs quick support for anxiety before work.',now()+interval '20 minutes',now(),now()),
        ('70000000-0000-0000-0000-000000000002','60000000-0000-0000-0000-000000000002','50000000-0000-0000-0000-000000000001','20000000-0000-0000-0000-000000000001','40000000-0000-0000-0000-000000000003','OnlineConsultation','Open','Patient requested online consultation with preferred doctor.',now()+interval '30 minutes',now(),now())
        ON CONFLICT ("Id") DO NOTHING;
        """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
        DELETE FROM portal.chat_session_jobs WHERE "Id" IN ('70000000-0000-0000-0000-000000000001','70000000-0000-0000-0000-000000000002');
        DELETE FROM portal.patient_accounts WHERE "Id" IN ('60000000-0000-0000-0000-000000000001','60000000-0000-0000-0000-000000000002');
        DELETE FROM portal.doctor_profiles WHERE "Id"='50000000-0000-0000-0000-000000000001';
        DELETE FROM portal.service_tiers WHERE "Id" IN ('40000000-0000-0000-0000-000000000001','40000000-0000-0000-0000-000000000002','40000000-0000-0000-0000-000000000003');
        DELETE FROM "AspNetUserClaims" WHERE "UserId" IN ('30000000-0000-0000-0000-000000000002','30000000-0000-0000-0000-000000000003');
        DELETE FROM "AspNetUserRoles" WHERE "UserId" IN ('30000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000002','30000000-0000-0000-0000-000000000003');
        DELETE FROM "AspNetUsers" WHERE "Id" IN ('30000000-0000-0000-0000-000000000001','30000000-0000-0000-0000-000000000002','30000000-0000-0000-0000-000000000003');
        DELETE FROM portal.clinics_hospitals WHERE "Id"='20000000-0000-0000-0000-000000000001';
        """);
    }
}
