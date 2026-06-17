using LIOSCare.ClinicPortal.Web.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIOSCare.ClinicPortal.Web.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260616190000_EnsureIdentitySeedDataSafe")]
public partial class EnsureIdentitySeedDataSafe : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
        CREATE SCHEMA IF NOT EXISTS portal;

        INSERT INTO "AspNetRoles"("Id", "Name", "NormalizedName", "ConcurrencyStamp", "Description") VALUES
        ('10000000-0000-0000-0000-000000000001','SuperAdmin','SUPERADMIN',gen_random_uuid()::text,'Full platform access'),
        ('10000000-0000-0000-0000-000000000002','Admin','ADMIN',gen_random_uuid()::text,'Clinic/Hospital administrator'),
        ('10000000-0000-0000-0000-000000000003','Doctor','DOCTOR',gen_random_uuid()::text,'Doctor workspace user')
        ON CONFLICT ("NormalizedName") DO UPDATE SET
            "Name" = excluded."Name",
            "Description" = excluded."Description";

        INSERT INTO "AspNetUsers"("Id","UserName","NormalizedUserName","Email","NormalizedEmail","EmailConfirmed","PasswordHash","SecurityStamp","ConcurrencyStamp","PhoneNumberConfirmed","TwoFactorEnabled","LockoutEnabled","AccessFailedCount","FullName","Status","ClinicHospitalId","CreatedAt","UpdatedAt") VALUES
        ('30000000-0000-0000-0000-000000000001','superadmin@lioscare.local','SUPERADMIN@LIOSCARE.LOCAL','superadmin@lioscare.local','SUPERADMIN@LIOSCARE.LOCAL',true,'AQAAAAIAAYagAAAAEAARIjNEVWZ3iJmqu8zd7v/GJJ7q0FjBnAAPOjfgGqVXBo8n0i0HkgsNod+cI+SwHg==',gen_random_uuid()::text,gen_random_uuid()::text,false,false,true,0,'System Super Admin','Active',NULL,now(),now()),
        ('30000000-0000-0000-0000-000000000002','clinicadmin@lioscare.local','CLINICADMIN@LIOSCARE.LOCAL','clinicadmin@lioscare.local','CLINICADMIN@LIOSCARE.LOCAL',true,'AQAAAAIAAYagAAAAEBERIiIzM0REVVVmZnd3iIh+RWFgAWaQ3IJewedWVPWIXNGJ+z67rNvBxzeuAJH46g==',gen_random_uuid()::text,gen_random_uuid()::text,false,false,true,0,'Demo Clinic Admin','Active',NULL,now(),now()),
        ('30000000-0000-0000-0000-000000000003','doctor@lioscare.local','DOCTOR@LIOSCARE.LOCAL','doctor@lioscare.local','DOCTOR@LIOSCARE.LOCAL',true,'AQAAAAIAAYagAAAAECIiMzNERFVVZmZ3d4iImZnb3gEVZGUC0VnCHJFDmaDsNgiUtEHFgI2SiWSn4/RJFA==',gen_random_uuid()::text,gen_random_uuid()::text,false,false,true,0,'Dr. Julia Adams','Active',NULL,now(),now())
        ON CONFLICT ("NormalizedUserName") DO UPDATE SET
            "Email" = excluded."Email",
            "NormalizedEmail" = excluded."NormalizedEmail",
            "EmailConfirmed" = true,
            "PasswordHash" = excluded."PasswordHash",
            "FullName" = excluded."FullName",
            "Status" = 'Active',
            "UpdatedAt" = now();

        INSERT INTO portal.clinics_hospitals("Id","Name","Type","Status","ContactEmail","ContactPhone","City","Country","Address","AdminUserId","CreatedAt","UpdatedAt")
        VALUES (
            '20000000-0000-0000-0000-000000000001',
            'LIOS Care Demo Clinic',
            'Clinic',
            'Approved',
            'clinicadmin@lioscare.local',
            '+1-555-0100',
            'New York',
            'United States',
            'Demo clinic address',
            (SELECT "Id" FROM "AspNetUsers" WHERE "NormalizedUserName"='CLINICADMIN@LIOSCARE.LOCAL' LIMIT 1),
            now(),
            now()
        )
        ON CONFLICT ("Id") DO UPDATE SET
            "Name" = excluded."Name",
            "Status" = 'Approved',
            "AdminUserId" = excluded."AdminUserId",
            "UpdatedAt" = now();

        UPDATE "AspNetUsers"
        SET "ClinicHospitalId" = '20000000-0000-0000-0000-000000000001', "UpdatedAt" = now()
        WHERE "NormalizedUserName" IN ('CLINICADMIN@LIOSCARE.LOCAL','DOCTOR@LIOSCARE.LOCAL');

        INSERT INTO "AspNetUserRoles"("UserId","RoleId")
        SELECT u."Id", r."Id" FROM "AspNetUsers" u JOIN "AspNetRoles" r ON r."NormalizedName"='SUPERADMIN'
        WHERE u."NormalizedUserName"='SUPERADMIN@LIOSCARE.LOCAL'
        ON CONFLICT DO NOTHING;

        INSERT INTO "AspNetUserRoles"("UserId","RoleId")
        SELECT u."Id", r."Id" FROM "AspNetUsers" u JOIN "AspNetRoles" r ON r."NormalizedName"='ADMIN'
        WHERE u."NormalizedUserName"='CLINICADMIN@LIOSCARE.LOCAL'
        ON CONFLICT DO NOTHING;

        INSERT INTO "AspNetUserRoles"("UserId","RoleId")
        SELECT u."Id", r."Id" FROM "AspNetUsers" u JOIN "AspNetRoles" r ON r."NormalizedName"='DOCTOR'
        WHERE u."NormalizedUserName"='DOCTOR@LIOSCARE.LOCAL'
        ON CONFLICT DO NOTHING;

        INSERT INTO "AspNetRoleClaims"("RoleId", "ClaimType", "ClaimValue")
        SELECT r."Id", v."ClaimType", v."ClaimValue"
        FROM "AspNetRoles" r
        JOIN (VALUES
            ('SUPERADMIN','permission','manage:super-admins'),
            ('SUPERADMIN','permission','manage:facilities'),
            ('SUPERADMIN','permission','manage:clinic-users'),
            ('SUPERADMIN','permission','manage:doctors'),
            ('SUPERADMIN','permission','manage:packages'),
            ('ADMIN','permission','manage:clinic-users'),
            ('ADMIN','permission','manage:doctors'),
            ('ADMIN','permission','manage:packages'),
            ('DOCTOR','permission','doctor:workspace'),
            ('DOCTOR','permission','view:notifications')
        ) AS v("Role", "ClaimType", "ClaimValue") ON v."Role" = r."NormalizedName"
        WHERE NOT EXISTS (
            SELECT 1 FROM "AspNetRoleClaims" c
            WHERE c."RoleId" = r."Id" AND c."ClaimType" = v."ClaimType" AND c."ClaimValue" = v."ClaimValue"
        );

        INSERT INTO "AspNetUserClaims"("UserId", "ClaimType", "ClaimValue")
        SELECT u."Id", 'clinic_id', '20000000-0000-0000-0000-000000000001'
        FROM "AspNetUsers" u
        WHERE u."NormalizedUserName" IN ('CLINICADMIN@LIOSCARE.LOCAL','DOCTOR@LIOSCARE.LOCAL')
        AND NOT EXISTS (
            SELECT 1 FROM "AspNetUserClaims" c
            WHERE c."UserId" = u."Id" AND c."ClaimType"='clinic_id' AND c."ClaimValue"='20000000-0000-0000-0000-000000000001'
        );

        INSERT INTO portal.service_tiers("Id","Name","DurationMinutes","Price","Currency","ServiceType","IsActive","SortOrder") VALUES
        ('40000000-0000-0000-0000-000000000001','Quick Chat 30',30,25,'USD','Chat',true,1),
        ('40000000-0000-0000-0000-000000000002','Quick Chat 40',40,35,'USD','Chat',true,2),
        ('40000000-0000-0000-0000-000000000003','Online Consultation 40',40,99,'USD','OnlineConsultation',true,3)
        ON CONFLICT ("Id") DO UPDATE SET
            "Price"=excluded."Price",
            "DurationMinutes"=excluded."DurationMinutes",
            "ServiceType"=excluded."ServiceType",
            "IsActive"=excluded."IsActive";

        INSERT INTO portal.doctor_profiles("Id","UserId","ClinicId","DisplayName","Specializations","YearsExperience","LicenseNumber","Bio","IsAvailable","MaxConcurrentSessions","Status","ChatSessionPrice","OnlineConsultationPrice","ChatSessionMinutes","OnlineConsultationMinutes","Currency","WorkingDays","WorkStartTime","WorkEndTime","CreatedAt","UpdatedAt")
        SELECT
            '50000000-0000-0000-0000-000000000001',
            u."Id",
            '20000000-0000-0000-0000-000000000001',
            'Dr. Julia Adams',
            ARRAY['Depression','Anxiety','Trauma'],
            10,
            'LIC-APYS-28491',
            'Licensed clinical psychologist focused on anxiety, trauma and practical therapy.',
            true,
            3,
            'Active',
            25,
            99,
            30,
            40,
            'USD',
            ARRAY['Monday','Tuesday','Wednesday','Thursday','Friday'],
            '09:00',
            '17:00',
            now(),
            now()
        FROM "AspNetUsers" u
        WHERE u."NormalizedUserName"='DOCTOR@LIOSCARE.LOCAL'
        ON CONFLICT ("Id") DO UPDATE SET
            "UserId"=excluded."UserId",
            "ClinicId"=excluded."ClinicId",
            "DisplayName"=excluded."DisplayName",
            "Status"='Active',
            "UpdatedAt"=now();

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
        // Seed hardening migration is intentionally non-destructive.
    }
}
