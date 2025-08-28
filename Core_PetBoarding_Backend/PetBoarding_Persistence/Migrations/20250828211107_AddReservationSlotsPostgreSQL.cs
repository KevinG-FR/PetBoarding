using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetBoarding_Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationSlotsPostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                schema: "PetBoarding",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentExpiryAt",
                schema: "PetBoarding",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "ReservationSlots",
                schema: "PetBoarding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReservationId = table.Column<Guid>(type: "uuid", maxLength: 36, nullable: false),
                    AvailableSlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReservedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    ReleasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationSlots_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalSchema: "PetBoarding",
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationSlots_Audit",
                schema: "PetBoarding",
                table: "ReservationSlots",
                columns: new[] { "ReleasedAt", "ReservedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationSlots_AvailableSlotId",
                schema: "PetBoarding",
                table: "ReservationSlots",
                column: "AvailableSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationSlots_ReservationId",
                schema: "PetBoarding",
                table: "ReservationSlots",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationSlots_ReservationId_AvailableSlotId",
                schema: "PetBoarding",
                table: "ReservationSlots",
                columns: new[] { "ReservationId", "AvailableSlotId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReservationSlots_ReservedAt",
                schema: "PetBoarding",
                table: "ReservationSlots",
                column: "ReservedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationSlots",
                schema: "PetBoarding");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                schema: "PetBoarding",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "PaymentExpiryAt",
                schema: "PetBoarding",
                table: "Reservations");
        }
    }
}
