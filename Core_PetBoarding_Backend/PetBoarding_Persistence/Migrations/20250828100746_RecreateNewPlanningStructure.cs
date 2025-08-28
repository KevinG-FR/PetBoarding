using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetBoarding_Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RecreateNewPlanningStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plannings",
                schema: "PetBoarding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PrestationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModification = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plannings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AvailableSlots",
                schema: "PetBoarding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlanningId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    MaxCapacity = table.Column<int>(type: "integer", nullable: false),
                    CapaciteReservee = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailableSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvailableSlots_Plannings_PlanningId",
                        column: x => x.PlanningId,
                        principalSchema: "PetBoarding",
                        principalTable: "Plannings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvailableSlots_Availability",
                schema: "PetBoarding",
                table: "AvailableSlots",
                columns: new[] { "Date", "MaxCapacity", "CapaciteReservee" });

            migrationBuilder.CreateIndex(
                name: "IX_AvailableSlots_Date",
                schema: "PetBoarding",
                table: "AvailableSlots",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_AvailableSlots_Planning_Date",
                schema: "PetBoarding",
                table: "AvailableSlots",
                columns: new[] { "PlanningId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plannings_Active_Prestation",
                schema: "PetBoarding",
                table: "Plannings",
                columns: new[] { "IsActive", "PrestationId" });

            migrationBuilder.CreateIndex(
                name: "IX_Plannings_IsActive",
                schema: "PetBoarding",
                table: "Plannings",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Plannings_PrestationId",
                schema: "PetBoarding",
                table: "Plannings",
                column: "PrestationId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvailableSlots",
                schema: "PetBoarding");

            migrationBuilder.DropTable(
                name: "Plannings",
                schema: "PetBoarding");
        }
    }
}
