using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetBoarding_Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReservationSlots_Audit",
                schema: "PetBoarding",
                table: "ReservationSlots");

            migrationBuilder.DropIndex(
                name: "IX_ReservationSlots_AvailableSlotId",
                schema: "PetBoarding",
                table: "ReservationSlots");

            migrationBuilder.DropIndex(
                name: "IX_ReservationSlots_ReservationId",
                schema: "PetBoarding",
                table: "ReservationSlots");

            migrationBuilder.DropIndex(
                name: "IX_ReservationSlots_ReservedAt",
                schema: "PetBoarding",
                table: "ReservationSlots");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ServiceId",
                schema: "PetBoarding",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_StartDate",
                schema: "PetBoarding",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_Status",
                schema: "PetBoarding",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_UserId",
                schema: "PetBoarding",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Prestations_CategorieAnimal",
                schema: "PetBoarding",
                table: "Prestations");

            migrationBuilder.DropIndex(
                name: "IX_Prestations_CategorieAnimal_EstDisponible",
                schema: "PetBoarding",
                table: "Prestations");

            migrationBuilder.DropIndex(
                name: "IX_Prestations_EstDisponible",
                schema: "PetBoarding",
                table: "Prestations");

            migrationBuilder.DropIndex(
                name: "IX_Pets_Type",
                schema: "PetBoarding",
                table: "Pets");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Status",
                schema: "PetBoarding",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Status_CreatedAt",
                schema: "PetBoarding",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_Status",
                schema: "PetBoarding",
                table: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_Status_CreatedAt",
                schema: "PetBoarding",
                table: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_UserId",
                schema: "PetBoarding",
                table: "Baskets");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationSlots_ReservationId_AvailableSlotId",
                schema: "PetBoarding",
                table: "ReservationSlots",
                newName: "idx_reservation_slots_reservation_available");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_DateRange",
                schema: "PetBoarding",
                table: "Reservations",
                newName: "idx_reservations_date_range");

            migrationBuilder.RenameIndex(
                name: "IX_Pets_OwnerId_Type",
                schema: "PetBoarding",
                table: "Pets",
                newName: "idx_pets_owner_type");

            migrationBuilder.RenameIndex(
                name: "IX_Pets_OwnerId",
                schema: "PetBoarding",
                table: "Pets",
                newName: "idx_pets_owner_id");

            migrationBuilder.RenameIndex(
                name: "IX_Pets_Name",
                schema: "PetBoarding",
                table: "Pets",
                newName: "idx_pets_name");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_ExternalTransactionId",
                schema: "PetBoarding",
                table: "Payments",
                newName: "idx_payments_external_transaction");

            migrationBuilder.RenameIndex(
                name: "IX_Baskets_PaymentId",
                schema: "PetBoarding",
                table: "Baskets",
                newName: "idx_baskets_payment_id");

            migrationBuilder.CreateIndex(
                name: "idx_users_created_at",
                schema: "PetBoarding",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "idx_users_email",
                schema: "PetBoarding",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "idx_users_email_password",
                schema: "PetBoarding",
                table: "Users",
                columns: new[] { "Email", "PasswordHash" });

            migrationBuilder.CreateIndex(
                name: "idx_users_status_profiletype",
                schema: "PetBoarding",
                table: "Users",
                columns: new[] { "Status", "ProfileType" });

            migrationBuilder.CreateIndex(
                name: "idx_reservation_slots_active",
                schema: "PetBoarding",
                table: "ReservationSlots",
                column: "ReleasedAt",
                filter: "\"ReleasedAt\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "idx_reservation_slots_available_active",
                schema: "PetBoarding",
                table: "ReservationSlots",
                columns: new[] { "AvailableSlotId", "ReleasedAt" });

            migrationBuilder.CreateIndex(
                name: "idx_reservations_serviceid_createdat",
                schema: "PetBoarding",
                table: "Reservations",
                columns: new[] { "ServiceId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "idx_reservations_status_startdate",
                schema: "PetBoarding",
                table: "Reservations",
                columns: new[] { "Status", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "idx_reservations_user_displayed",
                schema: "PetBoarding",
                table: "Reservations",
                columns: new[] { "UserId", "Status", "CreatedAt" },
                descending: new[] { false, false, true },
                filter: "\"Status\" IN ('Validated', 'InProgress', 'Completed')");

            migrationBuilder.CreateIndex(
                name: "idx_reservations_userid_createdat",
                schema: "PetBoarding",
                table: "Reservations",
                columns: new[] { "UserId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "idx_prestations_categorie_libelle",
                schema: "PetBoarding",
                table: "Prestations",
                columns: new[] { "CategorieAnimal", "Libelle" });

            migrationBuilder.CreateIndex(
                name: "idx_prestations_date_creation",
                schema: "PetBoarding",
                table: "Prestations",
                column: "DateCreation",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "idx_prestations_disponible",
                schema: "PetBoarding",
                table: "Prestations",
                column: "EstDisponible",
                filter: "\"EstDisponible\" = true");

            migrationBuilder.CreateIndex(
                name: "idx_prestations_disponible_categorie",
                schema: "PetBoarding",
                table: "Prestations",
                columns: new[] { "EstDisponible", "CategorieAnimal" });

            migrationBuilder.CreateIndex(
                name: "idx_payments_status_createdat",
                schema: "PetBoarding",
                table: "Payments",
                columns: new[] { "Status", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "idx_baskets_userid_status",
                schema: "PetBoarding",
                table: "Baskets",
                columns: new[] { "UserId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_users_created_at",
                schema: "PetBoarding",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "idx_users_email",
                schema: "PetBoarding",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "idx_users_email_password",
                schema: "PetBoarding",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "idx_users_status_profiletype",
                schema: "PetBoarding",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "idx_reservation_slots_active",
                schema: "PetBoarding",
                table: "ReservationSlots");

            migrationBuilder.DropIndex(
                name: "idx_reservation_slots_available_active",
                schema: "PetBoarding",
                table: "ReservationSlots");

            migrationBuilder.DropIndex(
                name: "idx_reservations_serviceid_createdat",
                schema: "PetBoarding",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "idx_reservations_status_startdate",
                schema: "PetBoarding",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "idx_reservations_user_displayed",
                schema: "PetBoarding",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "idx_reservations_userid_createdat",
                schema: "PetBoarding",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "idx_prestations_categorie_libelle",
                schema: "PetBoarding",
                table: "Prestations");

            migrationBuilder.DropIndex(
                name: "idx_prestations_date_creation",
                schema: "PetBoarding",
                table: "Prestations");

            migrationBuilder.DropIndex(
                name: "idx_prestations_disponible",
                schema: "PetBoarding",
                table: "Prestations");

            migrationBuilder.DropIndex(
                name: "idx_prestations_disponible_categorie",
                schema: "PetBoarding",
                table: "Prestations");

            migrationBuilder.DropIndex(
                name: "idx_payments_status_createdat",
                schema: "PetBoarding",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "idx_baskets_userid_status",
                schema: "PetBoarding",
                table: "Baskets");

            migrationBuilder.RenameIndex(
                name: "idx_reservation_slots_reservation_available",
                schema: "PetBoarding",
                table: "ReservationSlots",
                newName: "IX_ReservationSlots_ReservationId_AvailableSlotId");

            migrationBuilder.RenameIndex(
                name: "idx_reservations_date_range",
                schema: "PetBoarding",
                table: "Reservations",
                newName: "IX_Reservations_DateRange");

            migrationBuilder.RenameIndex(
                name: "idx_pets_owner_type",
                schema: "PetBoarding",
                table: "Pets",
                newName: "IX_Pets_OwnerId_Type");

            migrationBuilder.RenameIndex(
                name: "idx_pets_owner_id",
                schema: "PetBoarding",
                table: "Pets",
                newName: "IX_Pets_OwnerId");

            migrationBuilder.RenameIndex(
                name: "idx_pets_name",
                schema: "PetBoarding",
                table: "Pets",
                newName: "IX_Pets_Name");

            migrationBuilder.RenameIndex(
                name: "idx_payments_external_transaction",
                schema: "PetBoarding",
                table: "Payments",
                newName: "IX_Payments_ExternalTransactionId");

            migrationBuilder.RenameIndex(
                name: "idx_baskets_payment_id",
                schema: "PetBoarding",
                table: "Baskets",
                newName: "IX_Baskets_PaymentId");

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
                name: "IX_ReservationSlots_ReservedAt",
                schema: "PetBoarding",
                table: "ReservationSlots",
                column: "ReservedAt");

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
                name: "IX_Pets_Type",
                schema: "PetBoarding",
                table: "Pets",
                column: "Type");

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
        }
    }
}
