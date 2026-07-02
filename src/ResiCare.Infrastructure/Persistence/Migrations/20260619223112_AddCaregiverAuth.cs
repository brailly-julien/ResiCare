using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResiCare.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCaregiverAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Caregivers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Caregivers",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Caregivers_Email",
                table: "Caregivers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Caregivers_Email",
                table: "Caregivers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Caregivers");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Caregivers");
        }
    }
}
