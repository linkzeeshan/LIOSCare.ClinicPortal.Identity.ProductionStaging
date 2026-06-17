using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LIOSCare.ClinicPortal.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_AspNetUsers_TempId1",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "TempId1",
                table: "AspNetUsers",
                newName: "Id");

            migrationBuilder.AlterColumn<string[]>(
                name: "TechniquesUsed",
                schema: "portal",
                table: "session_reports",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnonymousPatientLabel",
                schema: "portal",
                table: "session_reports",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ChatSessionId",
                schema: "portal",
                table: "session_reports",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "portal",
                table: "session_reports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "GoalsCompleted",
                schema: "portal",
                table: "session_reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoalsTotal",
                schema: "portal",
                table: "session_reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockedAt",
                schema: "portal",
                table: "session_reports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "MoodAfter",
                schema: "portal",
                table: "session_reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MoodBefore",
                schema: "portal",
                table: "session_reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NextSteps",
                schema: "portal",
                table: "session_reports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProgressNotes",
                schema: "portal",
                table: "session_reports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "portal",
                table: "session_reports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                schema: "portal",
                table: "service_tiers",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                schema: "portal",
                table: "service_tiers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "portal",
                table: "service_tiers",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceType",
                schema: "portal",
                table: "service_tiers",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "portal",
                table: "reschedule_requests",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "portal",
                table: "reschedule_requests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "PatientId",
                schema: "portal",
                table: "reschedule_requests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                schema: "portal",
                table: "reschedule_requests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RequestedStartAt",
                schema: "portal",
                table: "reschedule_requests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RespondedAt",
                schema: "portal",
                table: "reschedule_requests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RespondedByDoctorId",
                schema: "portal",
                table: "reschedule_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AnonymousCode",
                schema: "portal",
                table: "patient_accounts",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnonymousDisplayName",
                schema: "portal",
                table: "patient_accounts",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "portal",
                table: "patient_accounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "UserPortalEmail",
                schema: "portal",
                table: "patient_accounts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Body",
                schema: "portal",
                table: "notifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                schema: "portal",
                table: "notifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                schema: "portal",
                table: "notifications",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "portal",
                table: "notifications",
                type: "character varying(180)",
                maxLength: 180,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "portal",
                table: "notifications",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string[]>(
                name: "WorkingDays",
                schema: "portal",
                table: "doctor_profiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "portal",
                table: "doctor_profiles",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string[]>(
                name: "Specializations",
                schema: "portal",
                table: "doctor_profiles",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                schema: "portal",
                table: "doctor_profiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ChatSessionMinutes",
                schema: "portal",
                table: "doctor_profiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "portal",
                table: "doctor_profiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                schema: "portal",
                table: "doctor_profiles",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                schema: "portal",
                table: "doctor_profiles",
                type: "character varying(140)",
                maxLength: 140,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                schema: "portal",
                table: "doctor_profiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                schema: "portal",
                table: "doctor_profiles",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaxConcurrentSessions",
                schema: "portal",
                table: "doctor_profiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OnlineConsultationMinutes",
                schema: "portal",
                table: "doctor_profiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "portal",
                table: "doctor_profiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "WorkEndTime",
                schema: "portal",
                table: "doctor_profiles",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "WorkStartTime",
                schema: "portal",
                table: "doctor_profiles",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearsExperience",
                schema: "portal",
                table: "doctor_profiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "portal",
                table: "clinics_hospitals",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "portal",
                table: "clinics_hospitals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "portal",
                table: "clinics_hospitals",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                schema: "portal",
                table: "clinics_hospitals",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactPhone",
                schema: "portal",
                table: "clinics_hospitals",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                schema: "portal",
                table: "clinics_hospitals",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "portal",
                table: "clinics_hospitals",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "portal",
                table: "clinics_hospitals",
                type: "character varying(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "portal",
                table: "clinics_hospitals",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "portal",
                table: "clinics_hospitals",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "portal",
                table: "chat_sessions",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ActualStartedAt",
                schema: "portal",
                table: "chat_sessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CloseReason",
                schema: "portal",
                table: "chat_sessions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ClosedAt",
                schema: "portal",
                table: "chat_sessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "portal",
                table: "chat_sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                schema: "portal",
                table: "chat_sessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ScheduledStartAt",
                schema: "portal",
                table: "chat_sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "portal",
                table: "chat_sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "portal",
                table: "chat_session_jobs",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AcceptedAt",
                schema: "portal",
                table: "chat_session_jobs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AcceptedByDoctorId",
                schema: "portal",
                table: "chat_session_jobs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                schema: "portal",
                table: "chat_session_jobs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "JobType",
                schema: "portal",
                table: "chat_session_jobs",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientNotePublic",
                schema: "portal",
                table: "chat_session_jobs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RejectedAt",
                schema: "portal",
                table: "chat_session_jobs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectedReason",
                schema: "portal",
                table: "chat_session_jobs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "portal",
                table: "chat_session_jobs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Body",
                schema: "portal",
                table: "chat_messages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderType",
                schema: "portal",
                table: "chat_messages",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "SenderUserId",
                schema: "portal",
                table: "chat_messages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AspNetUserTokens",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "AspNetUserTokens",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AspNetUsers",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true,
                oldDefaultValue: "Active");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "character varying(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(160)",
                oldMaxLength: 160,
                oldNullable: true,
                oldDefaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeactivatedAt",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AspNetUserRoles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "AspNetUserRoles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProviderDisplayName",
                table: "AspNetUserLogins",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AspNetUserLogins",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AspNetUserClaims",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "ClaimType",
                table: "AspNetUserClaims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClaimValue",
                table: "AspNetUserClaims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AspNetUserClaims",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "AspNetRoles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AspNetRoles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AspNetRoles",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetRoles",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "AspNetRoles",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "AspNetRoleClaims",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "ClaimType",
                table: "AspNetRoleClaims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClaimValue",
                table: "AspNetRoleClaims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "AspNetRoleClaims",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_session_reports_ChatSessionId",
                schema: "portal",
                table: "session_reports",
                column: "ChatSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_reschedule_requests_PatientId",
                schema: "portal",
                table: "reschedule_requests",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_sessions_JobId",
                schema: "portal",
                table: "chat_sessions",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_sessions_ServiceTierId",
                schema: "portal",
                table: "chat_sessions",
                column: "ServiceTierId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_session_jobs_PatientId",
                schema: "portal",
                table: "chat_session_jobs",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_chat_session_jobs_ServiceTierId",
                schema: "portal",
                table: "chat_session_jobs",
                column: "ServiceTierId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reschedule_requests_chat_sessions_ChatSessionId",
                schema: "portal",
                table: "reschedule_requests",
                column: "ChatSessionId",
                principalSchema: "portal",
                principalTable: "chat_sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reschedule_requests_patient_accounts_PatientId",
                schema: "portal",
                table: "reschedule_requests",
                column: "PatientId",
                principalSchema: "portal",
                principalTable: "patient_accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_session_reports_chat_sessions_ChatSessionId",
                schema: "portal",
                table: "session_reports",
                column: "ChatSessionId",
                principalSchema: "portal",
                principalTable: "chat_sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_session_reports_doctor_profiles_DoctorId",
                schema: "portal",
                table: "session_reports",
                column: "DoctorId",
                principalSchema: "portal",
                principalTable: "doctor_profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_reschedule_requests_chat_sessions_ChatSessionId",
                schema: "portal",
                table: "reschedule_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_reschedule_requests_patient_accounts_PatientId",
                schema: "portal",
                table: "reschedule_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_session_reports_chat_sessions_ChatSessionId",
                schema: "portal",
                table: "session_reports");

            migrationBuilder.DropForeignKey(
                name: "FK_session_reports_doctor_profiles_DoctorId",
                schema: "portal",
                table: "session_reports");

            migrationBuilder.DropIndex(
                name: "IX_session_reports_ChatSessionId",
                schema: "portal",
                table: "session_reports");

            migrationBuilder.DropIndex(
                name: "IX_reschedule_requests_PatientId",
                schema: "portal",
                table: "reschedule_requests");

            migrationBuilder.DropIndex(
                name: "IX_chat_sessions_JobId",
                schema: "portal",
                table: "chat_sessions");

            migrationBuilder.DropIndex(
                name: "IX_chat_sessions_ServiceTierId",
                schema: "portal",
                table: "chat_sessions");

            migrationBuilder.DropIndex(
                name: "IX_chat_session_jobs_PatientId",
                schema: "portal",
                table: "chat_session_jobs");

            migrationBuilder.DropIndex(
                name: "IX_chat_session_jobs_ServiceTierId",
                schema: "portal",
                table: "chat_session_jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                table: "AspNetUserLogins");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaims",
                table: "AspNetUserClaims");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoles",
                table: "AspNetRoles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaims",
                table: "AspNetRoleClaims");

            migrationBuilder.DropIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropColumn(
                name: "AnonymousPatientLabel",
                schema: "portal",
                table: "session_reports");

            migrationBuilder.DropColumn(
                name: "ChatSessionId",
                schema: "portal",
                table: "session_reports");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "portal",
                table: "session_reports");

            migrationBuilder.DropColumn(
                name: "GoalsCompleted",
                schema: "portal",
                table: "session_reports");

            migrationBuilder.DropColumn(
                name: "GoalsTotal",
                schema: "portal",
                table: "session_reports");

            migrationBuilder.DropColumn(
                name: "LockedAt",
                schema: "portal",
                table: "session_reports");

            migrationBuilder.DropColumn(
                name: "MoodAfter",
                schema: "portal",
                table: "session_reports");

            migrationBuilder.DropColumn(
                name: "MoodBefore",
                schema: "portal",
                table: "session_reports");

            migrationBuilder.DropColumn(
                name: "NextSteps",
                schema: "portal",
                table: "session_reports");

            migrationBuilder.DropColumn(
                name: "ProgressNotes",
                schema: "portal",
                table: "session_reports");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "portal",
                table: "session_reports");

            migrationBuilder.DropColumn(
                name: "Currency",
                schema: "portal",
                table: "service_tiers");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                schema: "portal",
                table: "service_tiers");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "portal",
                table: "service_tiers");

            migrationBuilder.DropColumn(
                name: "ServiceType",
                schema: "portal",
                table: "service_tiers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "portal",
                table: "reschedule_requests");

            migrationBuilder.DropColumn(
                name: "PatientId",
                schema: "portal",
                table: "reschedule_requests");

            migrationBuilder.DropColumn(
                name: "Reason",
                schema: "portal",
                table: "reschedule_requests");

            migrationBuilder.DropColumn(
                name: "RequestedStartAt",
                schema: "portal",
                table: "reschedule_requests");

            migrationBuilder.DropColumn(
                name: "RespondedAt",
                schema: "portal",
                table: "reschedule_requests");

            migrationBuilder.DropColumn(
                name: "RespondedByDoctorId",
                schema: "portal",
                table: "reschedule_requests");

            migrationBuilder.DropColumn(
                name: "AnonymousDisplayName",
                schema: "portal",
                table: "patient_accounts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "portal",
                table: "patient_accounts");

            migrationBuilder.DropColumn(
                name: "UserPortalEmail",
                schema: "portal",
                table: "patient_accounts");

            migrationBuilder.DropColumn(
                name: "Body",
                schema: "portal",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "EntityId",
                schema: "portal",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "EntityType",
                schema: "portal",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "portal",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "portal",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "Bio",
                schema: "portal",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "ChatSessionMinutes",
                schema: "portal",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "portal",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "Currency",
                schema: "portal",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                schema: "portal",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                schema: "portal",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                schema: "portal",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "MaxConcurrentSessions",
                schema: "portal",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "OnlineConsultationMinutes",
                schema: "portal",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "portal",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "WorkEndTime",
                schema: "portal",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "WorkStartTime",
                schema: "portal",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "YearsExperience",
                schema: "portal",
                table: "doctor_profiles");

            migrationBuilder.DropColumn(
                name: "Address",
                schema: "portal",
                table: "clinics_hospitals");

            migrationBuilder.DropColumn(
                name: "City",
                schema: "portal",
                table: "clinics_hospitals");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                schema: "portal",
                table: "clinics_hospitals");

            migrationBuilder.DropColumn(
                name: "ContactPhone",
                schema: "portal",
                table: "clinics_hospitals");

            migrationBuilder.DropColumn(
                name: "Country",
                schema: "portal",
                table: "clinics_hospitals");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "portal",
                table: "clinics_hospitals");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "portal",
                table: "clinics_hospitals");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "portal",
                table: "clinics_hospitals");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "portal",
                table: "clinics_hospitals");

            migrationBuilder.DropColumn(
                name: "ActualStartedAt",
                schema: "portal",
                table: "chat_sessions");

            migrationBuilder.DropColumn(
                name: "CloseReason",
                schema: "portal",
                table: "chat_sessions");

            migrationBuilder.DropColumn(
                name: "ClosedAt",
                schema: "portal",
                table: "chat_sessions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "portal",
                table: "chat_sessions");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                schema: "portal",
                table: "chat_sessions");

            migrationBuilder.DropColumn(
                name: "ScheduledStartAt",
                schema: "portal",
                table: "chat_sessions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "portal",
                table: "chat_sessions");

            migrationBuilder.DropColumn(
                name: "AcceptedAt",
                schema: "portal",
                table: "chat_session_jobs");

            migrationBuilder.DropColumn(
                name: "AcceptedByDoctorId",
                schema: "portal",
                table: "chat_session_jobs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "portal",
                table: "chat_session_jobs");

            migrationBuilder.DropColumn(
                name: "JobType",
                schema: "portal",
                table: "chat_session_jobs");

            migrationBuilder.DropColumn(
                name: "PatientNotePublic",
                schema: "portal",
                table: "chat_session_jobs");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                schema: "portal",
                table: "chat_session_jobs");

            migrationBuilder.DropColumn(
                name: "RejectedReason",
                schema: "portal",
                table: "chat_session_jobs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "portal",
                table: "chat_session_jobs");

            migrationBuilder.DropColumn(
                name: "Body",
                schema: "portal",
                table: "chat_messages");

            migrationBuilder.DropColumn(
                name: "SenderType",
                schema: "portal",
                table: "chat_messages");

            migrationBuilder.DropColumn(
                name: "SenderUserId",
                schema: "portal",
                table: "chat_messages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropColumn(
                name: "LoginProvider",
                table: "AspNetUserTokens");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUserTokens");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "AspNetUserTokens");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DeactivatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "LoginProvider",
                table: "AspNetUserLogins");

            migrationBuilder.DropColumn(
                name: "ProviderKey",
                table: "AspNetUserLogins");

            migrationBuilder.DropColumn(
                name: "ProviderDisplayName",
                table: "AspNetUserLogins");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AspNetUserClaims");

            migrationBuilder.DropColumn(
                name: "ClaimType",
                table: "AspNetUserClaims");

            migrationBuilder.DropColumn(
                name: "ClaimValue",
                table: "AspNetUserClaims");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "AspNetRoleClaims");

            migrationBuilder.DropColumn(
                name: "ClaimType",
                table: "AspNetRoleClaims");

            migrationBuilder.DropColumn(
                name: "ClaimValue",
                table: "AspNetRoleClaims");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "AspNetUsers",
                newName: "TempId1");

            migrationBuilder.AlterColumn<string[]>(
                name: "TechniquesUsed",
                schema: "portal",
                table: "session_reports",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "text[]");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "portal",
                table: "reschedule_requests",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "AnonymousCode",
                schema: "portal",
                table: "patient_accounts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string[]>(
                name: "WorkingDays",
                schema: "portal",
                table: "doctor_profiles",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "text[]");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "portal",
                table: "doctor_profiles",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string[]>(
                name: "Specializations",
                schema: "portal",
                table: "doctor_profiles",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "text[]");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "portal",
                table: "clinics_hospitals",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "portal",
                table: "chat_sessions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "portal",
                table: "chat_session_jobs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "AspNetUsers",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldDefaultValue: "Active");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "character varying(160)",
                maxLength: 160,
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(160)",
                oldMaxLength: 160,
                oldDefaultValue: "");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_AspNetUsers_TempId1",
                table: "AspNetUsers",
                column: "TempId1");
        }
    }
}
