using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Drones.Migrations
{
    /// <inheritdoc />
    public partial class SeedCustomerUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890") },
                column: "AssignedAt",
                value: new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$W3B1kNLF4KaTSpP2YgUVuOYiJ6/OSsFM5A5b7kXi7M37Avg1nVBcm" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "IsActive", "IsEmailVerified", "PasswordHash", "PhoneNumber", "UpdatedAt" },
                values: new object[] { new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"), new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), "customer@drones.com", true, true, "$2a$11$j0S.Y7eRq8ryzw7m0RsByOgB9yxFpICu8vmL3ZYQ0t4BIFRTZAg96", null, null });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedAt" },
                values: new object[] { 2, new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"), new DateTime(2026, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901") });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890") },
                column: "AssignedAt",
                value: new DateTime(2026, 6, 5, 10, 13, 2, 461, DateTimeKind.Utc).AddTicks(9302));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 5, 10, 13, 2, 461, DateTimeKind.Utc).AddTicks(8985), "$2a$11$7EgxOWPreiRYdOc7OZppf.xILl407QC/8Q4wz887qMba/vk3DniAm" });
        }
    }
}
