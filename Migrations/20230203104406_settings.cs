using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DayForceIntegration.Migrations
{
    /// <inheritdoc />
    public partial class settings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DayForcePassword",
                table: "settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DayForceUserName",
                table: "settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayForcePassword",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "DayForceUserName",
                table: "settings");
        }
    }
}
