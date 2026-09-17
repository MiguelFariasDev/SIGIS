using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sigis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCoverageEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                schema: "sigis",
                table: "professional",
                type: "character varying(254)",
                maxLength: 254,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "last_login_at",
                schema: "sigis",
                table: "professional",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "password_hash",
                schema: "sigis",
                table: "professional",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "attends_tutoring",
                schema: "sigis",
                table: "person",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "class_group",
                schema: "sigis",
                table: "person",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "current_school",
                schema: "sigis",
                table: "person",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "disability_types",
                schema: "sigis",
                table: "person",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "grade",
                schema: "sigis",
                table: "person",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "has_failed_grade",
                schema: "sigis",
                table: "person",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "naturality",
                schema: "sigis",
                table: "person",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "needs_special_education",
                schema: "sigis",
                table: "person",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "referred_by_school",
                schema: "sigis",
                table: "person",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "school_enrollment",
                schema: "sigis",
                table: "person",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "shift",
                schema: "sigis",
                table: "person",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "zone",
                schema: "sigis",
                table: "person",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "main_complaint",
                schema: "sigis",
                table: "attendance",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "triaged_by_professional_id",
                schema: "sigis",
                table: "attendance",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "concurrent_treatment",
                schema: "sigis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    specialty = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    location = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    professional_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    day_of_week = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    start_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    end_time = table.Column<TimeOnly>(type: "time", nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_concurrent_treatment", x => x.id);
                    table.ForeignKey(
                        name: "fk_concurrent_treatment_person_person_id",
                        column: x => x.person_id,
                        principalSchema: "sigis",
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "development_milestones",
                schema: "sigis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    age_walked_months = table.Column<int>(type: "integer", nullable: true),
                    age_talked_months = table.Column<int>(type: "integer", nullable: true),
                    locomotion_difficulty = table.Column<bool>(type: "boolean", nullable: true),
                    coordination_difficulty = table.Column<bool>(type: "boolean", nullable: true),
                    visual_difficulty = table.Column<bool>(type: "boolean", nullable: true),
                    hearing_difficulty = table.Column<bool>(type: "boolean", nullable: true),
                    speech_problems = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    command_comprehension = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    communication_form = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    manual_dominance = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_development_milestones", x => x.id);
                    table.ForeignKey(
                        name: "fk_development_milestones_person_person_id",
                        column: x => x.person_id,
                        principalSchema: "sigis",
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "family_composition",
                schema: "sigis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    father_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    father_education = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    father_occupation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    mother_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    mother_education = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    mother_occupation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    siblings_count = table.Column<int>(type: "integer", nullable: true),
                    siblings_ages = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    household_members_count = table.Column<int>(type: "integer", nullable: true),
                    parents_marital_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    filiation_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    planned_pregnancy = table.Column<bool>(type: "boolean", nullable: true),
                    pregnancies_count = table.Column<int>(type: "integer", nullable: true),
                    abortions_count = table.Column<int>(type: "integer", nullable: true),
                    pregnancy_health_issue = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    delivery_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    medication_during_pregnancy = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_family_composition", x => x.id);
                    table.ForeignKey(
                        name: "fk_family_composition_person_person_id",
                        column: x => x.person_id,
                        principalSchema: "sigis",
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "learning_difficulties",
                schema: "sigis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    assessment_date = table.Column<DateOnly>(type: "date", nullable: true),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_learning_difficulties", x => x.id);
                    table.ForeignKey(
                        name: "fk_learning_difficulties_person_person_id",
                        column: x => x.person_id,
                        principalSchema: "sigis",
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "person_clinical_profile",
                schema: "sigis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    medical_record_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    clinical_hypothesis = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    aps_reference_unit_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_clinical_profile", x => x.id);
                    table.ForeignKey(
                        name: "fk_person_clinical_profile_person_person_id",
                        column: x => x.person_id,
                        principalSchema: "sigis",
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_person_clinical_profile_service_unit_aps_reference_unit_id",
                        column: x => x.aps_reference_unit_id,
                        principalSchema: "sigis",
                        principalTable: "service_unit",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "person_consent",
                schema: "sigis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    granted = table.Column<bool>(type: "boolean", nullable: false),
                    granted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    granted_by_guardian_id = table.Column<Guid>(type: "uuid", nullable: true),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    evidence = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_consent", x => x.id);
                    table.ForeignKey(
                        name: "fk_person_consent_guardian_granted_by_guardian_id",
                        column: x => x.granted_by_guardian_id,
                        principalSchema: "sigis",
                        principalTable: "guardian",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_person_consent_person_person_id",
                        column: x => x.person_id,
                        principalSchema: "sigis",
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "school_history",
                schema: "sigis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    school_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    grade = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    shift = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    class_group = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    school_year = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_professional_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_school_history", x => x.id);
                    table.ForeignKey(
                        name: "fk_school_history_person_person_id",
                        column: x => x.person_id,
                        principalSchema: "sigis",
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_professional_email",
                schema: "sigis",
                table: "professional",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_attendance_triaged_by",
                schema: "sigis",
                table: "attendance",
                column: "triaged_by_professional_id");

            migrationBuilder.CreateIndex(
                name: "idx_concurrent_treatment_person",
                schema: "sigis",
                table: "concurrent_treatment",
                columns: new[] { "person_id", "day_of_week" });

            migrationBuilder.CreateIndex(
                name: "idx_development_milestones_person",
                schema: "sigis",
                table: "development_milestones",
                column: "person_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_family_composition_person",
                schema: "sigis",
                table: "family_composition",
                column: "person_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_learning_difficulties_person",
                schema: "sigis",
                table: "learning_difficulties",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "idx_learning_difficulties_person_type",
                schema: "sigis",
                table: "learning_difficulties",
                columns: new[] { "person_id", "type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_person_clinical_profile_person",
                schema: "sigis",
                table: "person_clinical_profile",
                column: "person_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_person_clinical_profile_record_unique",
                schema: "sigis",
                table: "person_clinical_profile",
                columns: new[] { "aps_reference_unit_id", "medical_record_number" },
                unique: true,
                filter: "medical_record_number IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_person_consent_active_unique",
                schema: "sigis",
                table: "person_consent",
                columns: new[] { "person_id", "type" },
                unique: true,
                filter: "revoked_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "idx_person_consent_person",
                schema: "sigis",
                table: "person_consent",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_consent_granted_by_guardian_id",
                schema: "sigis",
                table: "person_consent",
                column: "granted_by_guardian_id");

            migrationBuilder.CreateIndex(
                name: "idx_school_history_active_unique",
                schema: "sigis",
                table: "school_history",
                column: "person_id",
                unique: true,
                filter: "status = 'Ativo'");

            migrationBuilder.CreateIndex(
                name: "idx_school_history_person",
                schema: "sigis",
                table: "school_history",
                columns: new[] { "person_id", "school_year" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "concurrent_treatment",
                schema: "sigis");

            migrationBuilder.DropTable(
                name: "development_milestones",
                schema: "sigis");

            migrationBuilder.DropTable(
                name: "family_composition",
                schema: "sigis");

            migrationBuilder.DropTable(
                name: "learning_difficulties",
                schema: "sigis");

            migrationBuilder.DropTable(
                name: "person_clinical_profile",
                schema: "sigis");

            migrationBuilder.DropTable(
                name: "person_consent",
                schema: "sigis");

            migrationBuilder.DropTable(
                name: "school_history",
                schema: "sigis");

            migrationBuilder.DropIndex(
                name: "idx_professional_email",
                schema: "sigis",
                table: "professional");

            migrationBuilder.DropIndex(
                name: "idx_attendance_triaged_by",
                schema: "sigis",
                table: "attendance");

            migrationBuilder.DropColumn(
                name: "email",
                schema: "sigis",
                table: "professional");

            migrationBuilder.DropColumn(
                name: "last_login_at",
                schema: "sigis",
                table: "professional");

            migrationBuilder.DropColumn(
                name: "password_hash",
                schema: "sigis",
                table: "professional");

            migrationBuilder.DropColumn(
                name: "attends_tutoring",
                schema: "sigis",
                table: "person");

            migrationBuilder.DropColumn(
                name: "class_group",
                schema: "sigis",
                table: "person");

            migrationBuilder.DropColumn(
                name: "current_school",
                schema: "sigis",
                table: "person");

            migrationBuilder.DropColumn(
                name: "disability_types",
                schema: "sigis",
                table: "person");

            migrationBuilder.DropColumn(
                name: "grade",
                schema: "sigis",
                table: "person");

            migrationBuilder.DropColumn(
                name: "has_failed_grade",
                schema: "sigis",
                table: "person");

            migrationBuilder.DropColumn(
                name: "naturality",
                schema: "sigis",
                table: "person");

            migrationBuilder.DropColumn(
                name: "needs_special_education",
                schema: "sigis",
                table: "person");

            migrationBuilder.DropColumn(
                name: "referred_by_school",
                schema: "sigis",
                table: "person");

            migrationBuilder.DropColumn(
                name: "school_enrollment",
                schema: "sigis",
                table: "person");

            migrationBuilder.DropColumn(
                name: "shift",
                schema: "sigis",
                table: "person");

            migrationBuilder.DropColumn(
                name: "zone",
                schema: "sigis",
                table: "person");

            migrationBuilder.DropColumn(
                name: "main_complaint",
                schema: "sigis",
                table: "attendance");

            migrationBuilder.DropColumn(
                name: "triaged_by_professional_id",
                schema: "sigis",
                table: "attendance");
        }
    }
}
