using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DayForceIntegration.Migrations
{
    /// <inheritdoc />
    public partial class employee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_employeeData_location_HomeOrganizationId",
                table: "employeeData");

            migrationBuilder.DropIndex(
                name: "IX_employeeData_HomeOrganizationId",
                table: "employeeData");

            migrationBuilder.DropColumn(
                name: "HomeOrganizationId",
                table: "employeeData");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HomeOrganizationId",
                table: "employeeData",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_employeeData_HomeOrganizationId",
                table: "employeeData",
                column: "HomeOrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_employeeData_location_HomeOrganizationId",
                table: "employeeData",
                column: "HomeOrganizationId",
                principalTable: "location",
                principalColumn: "Id");
        }
    }
}
