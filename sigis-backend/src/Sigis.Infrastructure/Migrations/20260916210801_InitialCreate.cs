using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sigis.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sigis");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,")
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,")
                .Annotation("Npgsql:PostgresExtension:unaccent", ",,");

            // A função unaccent() da extensão é marcada STABLE (depende de
            // configuração de dicionário de texto), e o PostgreSQL exige que
            // a expressão de uma coluna GENERATED seja IMMUTABLE. Este
            // wrapper fixa o dicionário "unaccent" explicitamente, tornando
            // o resultado determinístico e elegível para uso em
            // sigis.person.name_normalized (GENERATED ALWAYS AS ... STORED)
            // e em índices funcionais.
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION sigis.immutable_unaccent(text)
                RETURNS text AS
                $$
                    SELECT unaccent('unaccent', $1)
                $$
                LANGUAGE sql IMMUTABLE PARALLEL SAFE STRICT;
                """);

            migrationBuilder.CreateTable(
                name: "access_log",
                schema: "sigis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    professional_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    legal_basis = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    justification = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    is_cross_unit = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_access_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "attendance",
                schema: "sigis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    professional_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    session_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    form_data = table.Column<string>(type: "jsonb", maxLength: 500, nullable: true),
                    session_number = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attendance", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "duplicate_alert",
                schema: "sigis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id_1 = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id_2 = table.Column<Guid>(type: "uuid", nullable: false),
                    similarity_score = table.Column<double>(type: "double precision", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    resolved_by_professional_id = table.Column<Guid>(type: "uuid", nullable: true),
                    resolved_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_duplicate_alert", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "person",
                schema: "sigis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    cns = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cpf = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    mother_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    gender = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    race_color = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    neighborhood = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    state = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    zip_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    name_normalized = table.Column<string>(type: "text", maxLength: 500, nullable: true, computedColumnSql: "sigis.immutable_unaccent(lower(name))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "queue_entry",
                schema: "sigis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    specialty = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    entered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    consecutive_absences = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_queue_entry", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "referral",
                schema: "sigis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    origin_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    destination_unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    referral_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_referral", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "service_unit",
                schema: "sigis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    acronym = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    secretariat = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_service_unit", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "guardian",
                schema: "sigis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    cns = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: true),
                    relationship = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guardian", x => x.id);
                    table.ForeignKey(
                        name: "fk_guardian_person_person_id",
                        column: x => x.person_id,
                        principalSchema: "sigis",
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "professional",
                schema: "sigis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    specialty = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    unit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_professional", x => x.id);
                    table.ForeignKey(
                        name: "fk_professional_service_unit_unit_id",
                        column: x => x.unit_id,
                        principalSchema: "sigis",
                        principalTable: "service_unit",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "idx_access_log_date",
                schema: "sigis",
                table: "access_log",
                column: "date_time");

            migrationBuilder.CreateIndex(
                name: "idx_access_log_person",
                schema: "sigis",
                table: "access_log",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "idx_access_log_professional",
                schema: "sigis",
                table: "access_log",
                column: "professional_id");

            migrationBuilder.CreateIndex(
                name: "idx_attendance_person_date",
                schema: "sigis",
                table: "attendance",
                columns: new[] { "person_id", "date_time" });

            migrationBuilder.CreateIndex(
                name: "idx_attendance_professional",
                schema: "sigis",
                table: "attendance",
                column: "professional_id");

            migrationBuilder.CreateIndex(
                name: "idx_attendance_unit",
                schema: "sigis",
                table: "attendance",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "idx_duplicate_alert_pair",
                schema: "sigis",
                table: "duplicate_alert",
                columns: new[] { "person_id_1", "person_id_2" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_duplicate_alert_status",
                schema: "sigis",
                table: "duplicate_alert",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "idx_guardian_person",
                schema: "sigis",
                table: "guardian",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "idx_person_birth_date",
                schema: "sigis",
                table: "person",
                column: "birth_date");

            migrationBuilder.CreateIndex(
                name: "idx_person_cns",
                schema: "sigis",
                table: "person",
                column: "cns",
                unique: true,
                filter: "cns IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_person_cpf",
                schema: "sigis",
                table: "person",
                column: "cpf",
                unique: true,
                filter: "cpf IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "idx_person_name_trgm",
                schema: "sigis",
                table: "person",
                column: "name_normalized")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "idx_professional_unit",
                schema: "sigis",
                table: "professional",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "idx_queue_active_unique",
                schema: "sigis",
                table: "queue_entry",
                columns: new[] { "person_id", "unit_id" },
                unique: true,
                filter: "status IN ('Waiting', 'InAttendance')");

            migrationBuilder.CreateIndex(
                name: "idx_queue_unit_status",
                schema: "sigis",
                table: "queue_entry",
                columns: new[] { "unit_id", "status" });

            migrationBuilder.CreateIndex(
                name: "idx_referral_correlation",
                schema: "sigis",
                table: "referral",
                column: "correlation_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_referral_destination_status",
                schema: "sigis",
                table: "referral",
                columns: new[] { "destination_unit_id", "status" });

            migrationBuilder.CreateIndex(
                name: "idx_referral_person",
                schema: "sigis",
                table: "referral",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "idx_service_unit_acronym",
                schema: "sigis",
                table: "service_unit",
                column: "acronym",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "access_log",
                schema: "sigis");

            migrationBuilder.DropTable(
                name: "attendance",
                schema: "sigis");

            migrationBuilder.DropTable(
                name: "duplicate_alert",
                schema: "sigis");

            migrationBuilder.DropTable(
                name: "guardian",
                schema: "sigis");

            migrationBuilder.DropTable(
                name: "professional",
                schema: "sigis");

            migrationBuilder.DropTable(
                name: "queue_entry",
                schema: "sigis");

            migrationBuilder.DropTable(
                name: "referral",
                schema: "sigis");

            migrationBuilder.DropTable(
                name: "person",
                schema: "sigis");

            migrationBuilder.DropTable(
                name: "service_unit",
                schema: "sigis");

            migrationBuilder.Sql("DROP FUNCTION IF EXISTS sigis.immutable_unaccent(text);");
        }
    }
}
