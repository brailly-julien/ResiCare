using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResiCare.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimeEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaregiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClockInAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClockOutAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeEntries_Caregivers_CaregiverId",
                        column: x => x.CaregiverId,
                        principalTable: "Caregivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntries_CaregiverId_ClockInAt",
                table: "TimeEntries",
                columns: new[] { "CaregiverId", "ClockInAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeEntries");
        }
    }
}
