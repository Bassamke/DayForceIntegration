using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DayForceIntegration.Migrations
{
    /// <inheritdoc />
    public partial class employeedetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "location",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClockTransferCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    XRefCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LongName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "employeeData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    XRefCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeOrganizationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_employeeData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_employeeData_location_HomeOrganizationId",
                        column: x => x.HomeOrganizationId,
                        principalTable: "location",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_employeeData_HomeOrganizationId",
                table: "employeeData",
                column: "HomeOrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employeeData");

            migrationBuilder.DropTable(
                name: "location");
        }
    }
}
