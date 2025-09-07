using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetBoarding_Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_users_email",
                schema: "PetBoarding",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "idx_users_email",
                schema: "PetBoarding",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_users_email",
                schema: "PetBoarding",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "idx_users_email",
                schema: "PetBoarding",
                table: "Users",
                column: "Email");
        }
    }
}
