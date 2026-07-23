using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drones.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantApplicationsAndFlightRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerUserId",
                table: "Restaurants",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DroneRoutePoints",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DispatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DroneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    BatteryLevel = table.Column<double>(type: "float", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DroneRoutePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DroneRoutePoints_Dispatches_DispatchId",
                        column: x => x.DispatchId,
                        principalTable: "Dispatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TaxId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RestaurantName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpenTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    CloseTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantApplications_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RestaurantApplications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 3, "Manages an approved restaurant.", "RestaurantOwner" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                column: "PasswordHash",
                value: "$2a$11$zPHvAL7LHezLpxKasoBO.umMHC1U572RQ3NLOdItUP6k2evqf3SQ2");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
                column: "PasswordHash",
                value: "$2a$11$I./gHxSX1pALg7MmLv.wZOPERsBb0borKEVyLvC69AgVSMy8TfoJ2");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_OwnerUserId",
                table: "Restaurants",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DroneRoutePoints_DispatchId_RecordedAt",
                table: "DroneRoutePoints",
                columns: new[] { "DispatchId", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantApplications_RestaurantId",
                table: "RestaurantApplications",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantApplications_TaxId",
                table: "RestaurantApplications",
                column: "TaxId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantApplications_UserId",
                table: "RestaurantApplications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_Users_OwnerUserId",
                table: "Restaurants",
                column: "OwnerUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_Users_OwnerUserId",
                table: "Restaurants");

            migrationBuilder.DropTable(
                name: "DroneRoutePoints");

            migrationBuilder.DropTable(
                name: "RestaurantApplications");

            migrationBuilder.DropIndex(
                name: "IX_Restaurants_OwnerUserId",
                table: "Restaurants");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "Restaurants");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                column: "PasswordHash",
                value: "$2a$11$s6DD1Ukcwa6eNmhRGn9Q2.MQe0Y.mf8oB.6BZJ/YAUe6kh837XiSu");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
                column: "PasswordHash",
                value: "$2a$11$8C5w5dOCWhV4GfGWj0LsgOK9OPvveycehCeX66VOKJ97AQC9bWm8W");
        }
    }
}
