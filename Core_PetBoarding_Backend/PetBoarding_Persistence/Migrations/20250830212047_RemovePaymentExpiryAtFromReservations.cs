using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetBoarding_Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovePaymentExpiryAtFromReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketItems_Prestations_PrestationId",
                schema: "PetBoarding",
                table: "BasketItems");

            migrationBuilder.DropColumn(
                name: "PaymentExpiryAt",
                schema: "PetBoarding",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Quantity",
                schema: "PetBoarding",
                table: "BasketItems");

            migrationBuilder.RenameColumn(
                name: "PrestationId",
                schema: "PetBoarding",
                table: "BasketItems",
                newName: "ReservationId");

            migrationBuilder.RenameIndex(
                name: "IX_BasketItems_PrestationId",
                schema: "PetBoarding",
                table: "BasketItems",
                newName: "IX_BasketItems_ReservationId");

            migrationBuilder.RenameIndex(
                name: "IX_BasketItems_BasketId_PrestationId",
                schema: "PetBoarding",
                table: "BasketItems",
                newName: "IX_BasketItems_BasketId_ReservationId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaidAt",
                schema: "PetBoarding",
                table: "Reservations",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BasketItems_Reservations_ReservationId",
                schema: "PetBoarding",
                table: "BasketItems",
                column: "ReservationId",
                principalSchema: "PetBoarding",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketItems_Reservations_ReservationId",
                schema: "PetBoarding",
                table: "BasketItems");

            migrationBuilder.RenameColumn(
                name: "ReservationId",
                schema: "PetBoarding",
                table: "BasketItems",
                newName: "PrestationId");

            migrationBuilder.RenameIndex(
                name: "IX_BasketItems_ReservationId",
                schema: "PetBoarding",
                table: "BasketItems",
                newName: "IX_BasketItems_PrestationId");

            migrationBuilder.RenameIndex(
                name: "IX_BasketItems_BasketId_ReservationId",
                schema: "PetBoarding",
                table: "BasketItems",
                newName: "IX_BasketItems_BasketId_PrestationId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaidAt",
                schema: "PetBoarding",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentExpiryAt",
                schema: "PetBoarding",
                table: "Reservations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                schema: "PetBoarding",
                table: "BasketItems",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddForeignKey(
                name: "FK_BasketItems_Prestations_PrestationId",
                schema: "PetBoarding",
                table: "BasketItems",
                column: "PrestationId",
                principalSchema: "PetBoarding",
                principalTable: "Prestations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
