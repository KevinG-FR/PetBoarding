using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetBoarding_Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBasketAndPaymentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "PetBoarding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ExternalTransactionId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailureReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Baskets",
                schema: "PetBoarding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PaymentFailureCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baskets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Baskets_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "PetBoarding",
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Baskets_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "PetBoarding",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BasketItems",
                schema: "PetBoarding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BasketId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrestationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    AddedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasketItems_Baskets_BasketId",
                        column: x => x.BasketId,
                        principalSchema: "PetBoarding",
                        principalTable: "Baskets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketItems_Prestations_PrestationId",
                        column: x => x.PrestationId,
                        principalSchema: "PetBoarding",
                        principalTable: "Prestations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_BasketId",
                schema: "PetBoarding",
                table: "BasketItems",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_BasketId_PrestationId",
                schema: "PetBoarding",
                table: "BasketItems",
                columns: new[] { "BasketId", "PrestationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_PrestationId",
                schema: "PetBoarding",
                table: "BasketItems",
                column: "PrestationId");

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_PaymentId",
                schema: "PetBoarding",
                table: "Baskets",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_Status",
                schema: "PetBoarding",
                table: "Baskets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_Status_CreatedAt",
                schema: "PetBoarding",
                table: "Baskets",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_UserId",
                schema: "PetBoarding",
                table: "Baskets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ExternalTransactionId",
                schema: "PetBoarding",
                table: "Payments",
                column: "ExternalTransactionId",
                unique: true,
                filter: "[ExternalTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status",
                schema: "PetBoarding",
                table: "Payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status_CreatedAt",
                schema: "PetBoarding",
                table: "Payments",
                columns: new[] { "Status", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BasketItems",
                schema: "PetBoarding");

            migrationBuilder.DropTable(
                name: "Baskets",
                schema: "PetBoarding");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "PetBoarding");
        }
    }
}
