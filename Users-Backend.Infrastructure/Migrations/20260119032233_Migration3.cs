using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Users_Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                columns: new[] { "CreateAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 1, 19, 3, 22, 32, 716, DateTimeKind.Utc).AddTicks(6367), "086736616e598710e984506abb07bb54d9f382103f145c9f1206247c2a61879d\n" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d3e4f5a6-b7c8-4d9e-0f1a-2b3c4d5e6f7a"),
                columns: new[] { "CreateAt", "PasswordHash", "UserName" },
                values: new object[] { new DateTime(2026, 1, 19, 3, 22, 32, 716, DateTimeKind.Utc).AddTicks(5752), "76ac3775bbadd88096d4aacef1ca7ac325fd1303dfac8f12d4462b3288cc5141\n", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                columns: new[] { "CreateAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 1, 14, 17, 47, 39, 678, DateTimeKind.Utc).AddTicks(3633), "123" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d3e4f5a6-b7c8-4d9e-0f1a-2b3c4d5e6f7a"),
                columns: new[] { "CreateAt", "PasswordHash", "UserName" },
                values: new object[] { new DateTime(2026, 1, 14, 17, 47, 39, 678, DateTimeKind.Utc).AddTicks(1485), "123", "Admin" });
        }
    }
}
