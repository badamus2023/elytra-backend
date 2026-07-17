using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Drones.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryPointId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeliveryPoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryPoints", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DeliveryPoints",
                columns: new[] { "Id", "Address", "CreatedAt", "IsActive", "Latitude", "Longitude", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c1d2e3f4-a5b6-7890-cdef-123456789abc"), "Al. Jerozolimskie 54, Warsaw", new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, 52.229700000000001, 21.0122, "Warsaw Central", null },
                    { new Guid("d2e3f4a5-b6c7-8901-defa-234567890bcd"), "Krakowskie Przedmieście 26/28, Warsaw", new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, 52.241100000000003, 21.0185, "University Campus", null },
                    { new Guid("e3f4a5b6-c7d8-9012-efab-345678901cde"), "Wołoska 12, Warsaw", new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), true, 52.193399999999997, 21.034500000000001, "Mokotów Park", null }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryPointId",
                table: "Orders",
                column: "DeliveryPointId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DeliveryPoints_DeliveryPointId",
                table: "Orders",
                column: "DeliveryPointId",
                principalTable: "DeliveryPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DeliveryPoints_DeliveryPointId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "DeliveryPoints");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DeliveryPointId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryPointId",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                column: "PasswordHash",
                value: "$2a$11$W3B1kNLF4KaTSpP2YgUVuOYiJ6/OSsFM5A5b7kXi7M37Avg1nVBcm");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
                column: "PasswordHash",
                value: "$2a$11$j0S.Y7eRq8ryzw7m0RsByOgB9yxFpICu8vmL3ZYQ0t4BIFRTZAg96");
        }
    }
}
