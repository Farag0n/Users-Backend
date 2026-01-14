using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Users_Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d3e4f5a6-b7c8-4d9e-0f1a-2b3c4d5e6f7a"),
                columns: new[] { "CreateAt", "Email", "LastName", "UserName" },
                values: new object[] { new DateTime(2026, 1, 14, 17, 47, 39, 678, DateTimeKind.Utc).AddTicks(1485), "admin@qwe.com", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreateAt", "Email", "IsDeleted", "LastName", "Name", "PasswordHash", "RefreshToken", "RefreshTokenExpiresDate", "UserName", "UserRole" },
                values: new object[] { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), new DateTime(2026, 1, 14, 17, 47, 39, 678, DateTimeKind.Utc).AddTicks(3633), "user@qwe.com", false, "User", "Test", "123", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d3e4f5a6-b7c8-4d9e-0f1a-2b3c4d5e6f7a"),
                columns: new[] { "CreateAt", "Email", "LastName", "UserName" },
                values: new object[] { new DateTime(2026, 1, 13, 9, 43, 24, 47, DateTimeKind.Utc).AddTicks(6458), "test@qwe.com", "User", "test" });
        }
    }
}
