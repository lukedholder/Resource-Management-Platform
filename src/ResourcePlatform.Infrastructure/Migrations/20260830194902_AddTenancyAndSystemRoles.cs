using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ResourcePlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenancyAndSystemRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "IsSystemRole", "Name", "OrganizationId" },
                values: new object[,]
                {
                    { new Guid("11111111-0000-0000-0000-000000000001"), true, "Owner", null },
                    { new Guid("11111111-0000-0000-0000-000000000002"), true, "Administrator", null },
                    { new Guid("11111111-0000-0000-0000-000000000003"), true, "Manager", null },
                    { new Guid("11111111-0000-0000-0000-000000000004"), true, "Member", null },
                    { new Guid("11111111-0000-0000-0000-000000000005"), true, "Viewer", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-0000-0000-0000-000000000005"));
        }
    }
}
