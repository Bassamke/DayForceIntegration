using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DayForceIntegration.Migrations
{
    /// <inheritdoc />
    public partial class employeePrimaryAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrimaryAdress",
                table: "employeeData",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryAdress",
                table: "employeeData");
        }
    }
}
