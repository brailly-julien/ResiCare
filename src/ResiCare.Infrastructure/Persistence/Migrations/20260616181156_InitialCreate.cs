using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResiCare.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Caregivers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caregivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Residents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AdmissionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DependencyLevel = table.Column<int>(type: "int", nullable: false),
                    ReferentCaregiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Residents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Residents_Caregivers_ReferentCaregiverId",
                        column: x => x.ReferentCaregiverId,
                        principalTable: "Caregivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CareTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CompletedByCaregiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CareTasks_Caregivers_CompletedByCaregiverId",
                        column: x => x.CompletedByCaregiverId,
                        principalTable: "Caregivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CareTasks_Residents_ResidentId",
                        column: x => x.ResidentId,
                        principalTable: "Residents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Observations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResidentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CaregiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Observations_Caregivers_CaregiverId",
                        column: x => x.CaregiverId,
                        principalTable: "Caregivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Observations_Residents_ResidentId",
                        column: x => x.ResidentId,
                        principalTable: "Residents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CareTasks_CompletedByCaregiverId",
                table: "CareTasks",
                column: "CompletedByCaregiverId");

            migrationBuilder.CreateIndex(
                name: "IX_CareTasks_ResidentId_ScheduledDate",
                table: "CareTasks",
                columns: new[] { "ResidentId", "ScheduledDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Observations_CaregiverId",
                table: "Observations",
                column: "CaregiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Observations_ResidentId_CreatedAt",
                table: "Observations",
                columns: new[] { "ResidentId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Residents_LastName_FirstName",
                table: "Residents",
                columns: new[] { "LastName", "FirstName" });

            migrationBuilder.CreateIndex(
                name: "IX_Residents_ReferentCaregiverId",
                table: "Residents",
                column: "ReferentCaregiverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CareTasks");

            migrationBuilder.DropTable(
                name: "Observations");

            migrationBuilder.DropTable(
                name: "Residents");

            migrationBuilder.DropTable(
                name: "Caregivers");
        }
    }
}
