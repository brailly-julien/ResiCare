using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResiCare.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnrichResidentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DependencyLevel",
                table: "Residents",
                newName: "Dependency_Mobility");

            migrationBuilder.AddColumn<string>(
                name: "AttendingPhysician",
                table: "Residents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Dependency_Dressing",
                table: "Residents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Dependency_Eating",
                table: "Residents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Dependency_Elimination",
                table: "Residents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Dependency_Hygiene",
                table: "Residents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactName",
                table: "Residents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactPhone",
                table: "Residents",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FallRisk",
                table: "Residents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Family",
                table: "Residents",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Interests",
                table: "Residents",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MalnutritionRisk",
                table: "Residents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Occupation",
                table: "Residents",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PressureSoreRisk",
                table: "Residents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttendingPhysician",
                table: "Residents");

            migrationBuilder.DropColumn(
                name: "Dependency_Dressing",
                table: "Residents");

            migrationBuilder.DropColumn(
                name: "Dependency_Eating",
                table: "Residents");

            migrationBuilder.DropColumn(
                name: "Dependency_Elimination",
                table: "Residents");

            migrationBuilder.DropColumn(
                name: "Dependency_Hygiene",
                table: "Residents");

            migrationBuilder.DropColumn(
                name: "EmergencyContactName",
                table: "Residents");

            migrationBuilder.DropColumn(
                name: "EmergencyContactPhone",
                table: "Residents");

            migrationBuilder.DropColumn(
                name: "FallRisk",
                table: "Residents");

            migrationBuilder.DropColumn(
                name: "Family",
                table: "Residents");

            migrationBuilder.DropColumn(
                name: "Interests",
                table: "Residents");

            migrationBuilder.DropColumn(
                name: "MalnutritionRisk",
                table: "Residents");

            migrationBuilder.DropColumn(
                name: "Occupation",
                table: "Residents");

            migrationBuilder.DropColumn(
                name: "PressureSoreRisk",
                table: "Residents");

            migrationBuilder.RenameColumn(
                name: "Dependency_Mobility",
                table: "Residents",
                newName: "DependencyLevel");
        }
    }
}
