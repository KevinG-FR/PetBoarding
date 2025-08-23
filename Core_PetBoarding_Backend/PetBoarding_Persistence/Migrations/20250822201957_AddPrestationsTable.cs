using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetBoarding_Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPrestationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prestations",
                schema: "PetBoarding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Libelle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CategorieAnimal = table.Column<int>(type: "integer", nullable: false),
                    Prix = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    DureeEnMinutes = table.Column<int>(type: "integer", nullable: false),
                    EstDisponible = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModification = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                schema: "PetBoarding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AnimalId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AnimalName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ServiceId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    Comments = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TotalPrice = table.Column<decimal>(type: "numeric(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prestations_CategorieAnimal",
                schema: "PetBoarding",
                table: "Prestations",
                column: "CategorieAnimal");

            migrationBuilder.CreateIndex(
                name: "IX_Prestations_CategorieAnimal_EstDisponible",
                schema: "PetBoarding",
                table: "Prestations",
                columns: new[] { "CategorieAnimal", "EstDisponible" });

            migrationBuilder.CreateIndex(
                name: "IX_Prestations_EstDisponible",
                schema: "PetBoarding",
                table: "Prestations",
                column: "EstDisponible");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_DateRange",
                schema: "PetBoarding",
                table: "Reservations",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ServiceId",
                schema: "PetBoarding",
                table: "Reservations",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_StartDate",
                schema: "PetBoarding",
                table: "Reservations",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_Status",
                schema: "PetBoarding",
                table: "Reservations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_UserId",
                schema: "PetBoarding",
                table: "Reservations",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prestations",
                schema: "PetBoarding");

            migrationBuilder.DropTable(
                name: "Reservations",
                schema: "PetBoarding");
        }
    }
}
