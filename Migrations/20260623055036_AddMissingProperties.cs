using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticaApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BulkDiscount",
                table: "Tariffs");

            migrationBuilder.DropColumn(
                name: "EffectiveDate",
                table: "Tariffs");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Tariffs");

            migrationBuilder.DropColumn(
                name: "LoyaltyDiscount",
                table: "Tariffs");

            migrationBuilder.DropColumn(
                name: "PricePerKm",
                table: "Tariffs");

            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "CurrentLatitude",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "CurrentLongitude",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "LastLocationUpdate",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "SuccessfulDeliveries",
                table: "Drivers");

            migrationBuilder.RenameColumn(
                name: "UrgentSurcharge",
                table: "Tariffs",
                newName: "MinDistance");

            migrationBuilder.RenameColumn(
                name: "SpecialHandlingSurcharge",
                table: "Tariffs",
                newName: "MaxDistance");

            migrationBuilder.RenameColumn(
                name: "CancelledAt",
                table: "Shipments",
                newName: "AssignedAt");

            migrationBuilder.RenameColumn(
                name: "ReportedAt",
                table: "Incidents",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Tariffs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPriority",
                table: "Tariffs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Tariffs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DeliveryId",
                table: "Shipments",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IncidentType",
                table: "Incidents",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "DriverId1",
                table: "Incidents",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Drivers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_DriverId1",
                table: "Incidents",
                column: "DriverId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Drivers_DriverId1",
                table: "Incidents",
                column: "DriverId1",
                principalTable: "Drivers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Drivers_DriverId1",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_DriverId1",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "IsPriority",
                table: "Tariffs");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Tariffs");

            migrationBuilder.DropColumn(
                name: "DeliveryId",
                table: "Shipments");

            migrationBuilder.DropColumn(
                name: "DriverId1",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Drivers");

            migrationBuilder.RenameColumn(
                name: "MinDistance",
                table: "Tariffs",
                newName: "UrgentSurcharge");

            migrationBuilder.RenameColumn(
                name: "MaxDistance",
                table: "Tariffs",
                newName: "SpecialHandlingSurcharge");

            migrationBuilder.RenameColumn(
                name: "AssignedAt",
                table: "Shipments",
                newName: "CancelledAt");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Incidents",
                newName: "ReportedAt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Tariffs",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<decimal>(
                name: "BulkDiscount",
                table: "Tariffs",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveDate",
                table: "Tariffs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "Tariffs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LoyaltyDiscount",
                table: "Tariffs",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerKm",
                table: "Tariffs",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "Shipments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "IncidentType",
                table: "Incidents",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Incidents",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Incidents",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CurrentLatitude",
                table: "Drivers",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentLongitude",
                table: "Drivers",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLocationUpdate",
                table: "Drivers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SuccessfulDeliveries",
                table: "Drivers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
