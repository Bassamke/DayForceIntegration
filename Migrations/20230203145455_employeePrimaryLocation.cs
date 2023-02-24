using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DayForceIntegration.Migrations
{
    /// <inheritdoc />
    public partial class employeePrimaryLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrimaryAdress",
                table: "employeeData",
                newName: "PrimaryLocation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrimaryLocation",
                table: "employeeData",
                newName: "PrimaryAdress");
        }
    }
}
