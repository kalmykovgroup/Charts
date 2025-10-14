using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Charts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "databases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    database_version = table.Column<string>(type: "text", nullable: false),
                    connection_string = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    provider = table.Column<int>(type: "integer", nullable: false),
                    database_status = table.Column<int>(type: "integer", nullable: false),
                    availability = table.Column<int>(type: "integer", nullable: false),
                    last_connectivity_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_connectivity_error = table.Column<string>(type: "text", nullable: true),
                    entities = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_databases", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chart_req_templates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    database_id = table.Column<Guid>(type: "uuid", nullable: false),
                    original_from_ms = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    original_to_ms = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    entity = table.Column<string>(type: "jsonb", nullable: false),
                    time_field = table.Column<string>(type: "jsonb", nullable: false),
                    selected_fields = table.Column<string>(type: "jsonb", nullable: false),
                    where = table.Column<string>(type: "jsonb", nullable: true),
                    sql = table.Column<string>(type: "text", nullable: true),
                    @params = table.Column<string>(name: "params", type: "jsonb", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chart_req_templates", x => x.id);
                    table.ForeignKey(
                        name: "fk_chart_req_templates_databases_database_id",
                        column: x => x.database_id,
                        principalTable: "databases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_chart_template_dbid_entity_time",
                table: "chart_req_templates",
                columns: new[] { "database_id", "entity", "time_field" });

            migrationBuilder.CreateIndex(
                name: "ux_chart_template_dbid_name",
                table: "chart_req_templates",
                columns: new[] { "database_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_databases_name",
                table: "databases",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chart_req_templates");

            migrationBuilder.DropTable(
                name: "databases");
        }
    }
}
