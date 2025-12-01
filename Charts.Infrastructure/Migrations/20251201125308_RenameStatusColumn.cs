using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Charts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "database_status",
                table: "databases",
                newName: "status");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "chart_req_templates",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "chart_req_templates");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "databases",
                newName: "database_status");
        }
    }
}
