using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResiCare.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeEntryOpenShiftUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UX_TimeEntries_CaregiverId_OpenShift",
                table: "TimeEntries",
                column: "CaregiverId",
                unique: true,
                filter: "ClockOutAt IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_TimeEntries_CaregiverId_OpenShift",
                table: "TimeEntries");
        }
    }
}
