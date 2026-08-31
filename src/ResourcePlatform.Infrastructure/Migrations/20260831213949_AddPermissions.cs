using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ResourcePlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("049b4483-ce6a-35c1-c766-2601f8552cef"), null, "Location.Create" },
                    { new Guid("1ae7500c-7a1c-58c5-1fcf-920141b79c57"), null, "ResourceType.Delete" },
                    { new Guid("1e71a3fa-0d38-b71a-18a6-0ca32ed8de0b"), null, "ResourceType.Update" },
                    { new Guid("3682e3d3-4ff6-8c0d-72ab-ba20bc58e31d"), null, "Reservation.Create" },
                    { new Guid("4091df6a-0088-e339-8960-a9d2a5910c99"), null, "Organization.Delete" },
                    { new Guid("455c5b15-5934-c2b1-d7c1-cd0daafbede1"), null, "Location.Update" },
                    { new Guid("53a9485b-0b9a-888c-8f80-73ec7b8d4c84"), null, "Reservation.Read" },
                    { new Guid("556ec1b1-7aeb-7091-51b2-3f00f194ae80"), null, "Resource.Read" },
                    { new Guid("6f17ae43-ba84-9a2a-2c16-10223715ff7b"), null, "Location.Delete" },
                    { new Guid("7347c061-93f9-2981-b53d-d999ce39d021"), null, "Resource.Delete" },
                    { new Guid("79e23999-58c4-8219-234b-a6a3a9172e92"), null, "Reservation.CancelOwn" },
                    { new Guid("8c0b81b0-84c5-b25b-8699-6ee51997f255"), null, "Location.Read" },
                    { new Guid("95185dfc-4aea-c4d8-66c5-77235e0c3075"), null, "Member.AssignRole" },
                    { new Guid("b5dfd67b-408e-aa3b-0c71-d79be7fab61f"), null, "Reservation.CancelAny" },
                    { new Guid("be48da10-f3dc-48e1-2112-a8eb0002f8ba"), null, "Member.Invite" },
                    { new Guid("be94b080-b979-2550-66ac-8cb84826e55a"), null, "ResourceType.Create" },
                    { new Guid("c8ace02b-0da5-71ee-e024-22cd3e485a9c"), null, "Resource.Update" },
                    { new Guid("d125ccf7-474b-e193-0883-221b06dea83b"), null, "Resource.Create" },
                    { new Guid("e5d436e7-73e7-09ca-d02b-fb337d790ad9"), null, "Reservation.Approve" },
                    { new Guid("ed4f3f7f-eb00-1e61-003c-d0cb353bb39d"), null, "Audit.Read" },
                    { new Guid("f38228da-566d-1152-2852-5a0a1a57368e"), null, "Member.Read" },
                    { new Guid("f911858b-88cd-7c16-f923-3c0a8c7b0260"), null, "ResourceType.Read" },
                    { new Guid("fe0751d0-fdf4-6e60-5dda-b45f5e4967f9"), null, "Member.Remove" },
                    { new Guid("ff95a46b-d616-d6a2-ddba-21506fc963f2"), null, "Organization.Update" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("049b4483-ce6a-35c1-c766-2601f8552cef"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("1ae7500c-7a1c-58c5-1fcf-920141b79c57"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("1e71a3fa-0d38-b71a-18a6-0ca32ed8de0b"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("3682e3d3-4ff6-8c0d-72ab-ba20bc58e31d"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("4091df6a-0088-e339-8960-a9d2a5910c99"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("455c5b15-5934-c2b1-d7c1-cd0daafbede1"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("53a9485b-0b9a-888c-8f80-73ec7b8d4c84"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("556ec1b1-7aeb-7091-51b2-3f00f194ae80"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("6f17ae43-ba84-9a2a-2c16-10223715ff7b"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("7347c061-93f9-2981-b53d-d999ce39d021"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("79e23999-58c4-8219-234b-a6a3a9172e92"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("8c0b81b0-84c5-b25b-8699-6ee51997f255"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("95185dfc-4aea-c4d8-66c5-77235e0c3075"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("b5dfd67b-408e-aa3b-0c71-d79be7fab61f"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("be48da10-f3dc-48e1-2112-a8eb0002f8ba"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("be94b080-b979-2550-66ac-8cb84826e55a"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("c8ace02b-0da5-71ee-e024-22cd3e485a9c"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("d125ccf7-474b-e193-0883-221b06dea83b"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("e5d436e7-73e7-09ca-d02b-fb337d790ad9"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("ed4f3f7f-eb00-1e61-003c-d0cb353bb39d"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("f38228da-566d-1152-2852-5a0a1a57368e"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("f911858b-88cd-7c16-f923-3c0a8c7b0260"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("fe0751d0-fdf4-6e60-5dda-b45f5e4967f9"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("ff95a46b-d616-d6a2-ddba-21506fc963f2"), new Guid("11111111-0000-0000-0000-000000000001") },
                    { new Guid("049b4483-ce6a-35c1-c766-2601f8552cef"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("1ae7500c-7a1c-58c5-1fcf-920141b79c57"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("1e71a3fa-0d38-b71a-18a6-0ca32ed8de0b"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("3682e3d3-4ff6-8c0d-72ab-ba20bc58e31d"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("455c5b15-5934-c2b1-d7c1-cd0daafbede1"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("53a9485b-0b9a-888c-8f80-73ec7b8d4c84"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("556ec1b1-7aeb-7091-51b2-3f00f194ae80"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("6f17ae43-ba84-9a2a-2c16-10223715ff7b"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("7347c061-93f9-2981-b53d-d999ce39d021"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("79e23999-58c4-8219-234b-a6a3a9172e92"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("8c0b81b0-84c5-b25b-8699-6ee51997f255"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("95185dfc-4aea-c4d8-66c5-77235e0c3075"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("b5dfd67b-408e-aa3b-0c71-d79be7fab61f"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("be48da10-f3dc-48e1-2112-a8eb0002f8ba"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("be94b080-b979-2550-66ac-8cb84826e55a"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("c8ace02b-0da5-71ee-e024-22cd3e485a9c"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("d125ccf7-474b-e193-0883-221b06dea83b"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("e5d436e7-73e7-09ca-d02b-fb337d790ad9"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("ed4f3f7f-eb00-1e61-003c-d0cb353bb39d"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("f38228da-566d-1152-2852-5a0a1a57368e"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("f911858b-88cd-7c16-f923-3c0a8c7b0260"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("fe0751d0-fdf4-6e60-5dda-b45f5e4967f9"), new Guid("11111111-0000-0000-0000-000000000002") },
                    { new Guid("3682e3d3-4ff6-8c0d-72ab-ba20bc58e31d"), new Guid("11111111-0000-0000-0000-000000000003") },
                    { new Guid("53a9485b-0b9a-888c-8f80-73ec7b8d4c84"), new Guid("11111111-0000-0000-0000-000000000003") },
                    { new Guid("556ec1b1-7aeb-7091-51b2-3f00f194ae80"), new Guid("11111111-0000-0000-0000-000000000003") },
                    { new Guid("79e23999-58c4-8219-234b-a6a3a9172e92"), new Guid("11111111-0000-0000-0000-000000000003") },
                    { new Guid("8c0b81b0-84c5-b25b-8699-6ee51997f255"), new Guid("11111111-0000-0000-0000-000000000003") },
                    { new Guid("b5dfd67b-408e-aa3b-0c71-d79be7fab61f"), new Guid("11111111-0000-0000-0000-000000000003") },
                    { new Guid("c8ace02b-0da5-71ee-e024-22cd3e485a9c"), new Guid("11111111-0000-0000-0000-000000000003") },
                    { new Guid("d125ccf7-474b-e193-0883-221b06dea83b"), new Guid("11111111-0000-0000-0000-000000000003") },
                    { new Guid("e5d436e7-73e7-09ca-d02b-fb337d790ad9"), new Guid("11111111-0000-0000-0000-000000000003") },
                    { new Guid("f38228da-566d-1152-2852-5a0a1a57368e"), new Guid("11111111-0000-0000-0000-000000000003") },
                    { new Guid("f911858b-88cd-7c16-f923-3c0a8c7b0260"), new Guid("11111111-0000-0000-0000-000000000003") },
                    { new Guid("3682e3d3-4ff6-8c0d-72ab-ba20bc58e31d"), new Guid("11111111-0000-0000-0000-000000000004") },
                    { new Guid("53a9485b-0b9a-888c-8f80-73ec7b8d4c84"), new Guid("11111111-0000-0000-0000-000000000004") },
                    { new Guid("556ec1b1-7aeb-7091-51b2-3f00f194ae80"), new Guid("11111111-0000-0000-0000-000000000004") },
                    { new Guid("79e23999-58c4-8219-234b-a6a3a9172e92"), new Guid("11111111-0000-0000-0000-000000000004") },
                    { new Guid("8c0b81b0-84c5-b25b-8699-6ee51997f255"), new Guid("11111111-0000-0000-0000-000000000004") },
                    { new Guid("f38228da-566d-1152-2852-5a0a1a57368e"), new Guid("11111111-0000-0000-0000-000000000004") },
                    { new Guid("f911858b-88cd-7c16-f923-3c0a8c7b0260"), new Guid("11111111-0000-0000-0000-000000000004") },
                    { new Guid("53a9485b-0b9a-888c-8f80-73ec7b8d4c84"), new Guid("11111111-0000-0000-0000-000000000005") },
                    { new Guid("556ec1b1-7aeb-7091-51b2-3f00f194ae80"), new Guid("11111111-0000-0000-0000-000000000005") },
                    { new Guid("8c0b81b0-84c5-b25b-8699-6ee51997f255"), new Guid("11111111-0000-0000-0000-000000000005") },
                    { new Guid("f38228da-566d-1152-2852-5a0a1a57368e"), new Guid("11111111-0000-0000-0000-000000000005") },
                    { new Guid("f911858b-88cd-7c16-f923-3c0a8c7b0260"), new Guid("11111111-0000-0000-0000-000000000005") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("049b4483-ce6a-35c1-c766-2601f8552cef"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("1ae7500c-7a1c-58c5-1fcf-920141b79c57"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("1e71a3fa-0d38-b71a-18a6-0ca32ed8de0b"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("3682e3d3-4ff6-8c0d-72ab-ba20bc58e31d"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("4091df6a-0088-e339-8960-a9d2a5910c99"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("455c5b15-5934-c2b1-d7c1-cd0daafbede1"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("53a9485b-0b9a-888c-8f80-73ec7b8d4c84"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("556ec1b1-7aeb-7091-51b2-3f00f194ae80"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("6f17ae43-ba84-9a2a-2c16-10223715ff7b"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("7347c061-93f9-2981-b53d-d999ce39d021"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("79e23999-58c4-8219-234b-a6a3a9172e92"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("8c0b81b0-84c5-b25b-8699-6ee51997f255"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("95185dfc-4aea-c4d8-66c5-77235e0c3075"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("b5dfd67b-408e-aa3b-0c71-d79be7fab61f"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("be48da10-f3dc-48e1-2112-a8eb0002f8ba"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("be94b080-b979-2550-66ac-8cb84826e55a"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("c8ace02b-0da5-71ee-e024-22cd3e485a9c"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("d125ccf7-474b-e193-0883-221b06dea83b"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("e5d436e7-73e7-09ca-d02b-fb337d790ad9"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("ed4f3f7f-eb00-1e61-003c-d0cb353bb39d"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("f38228da-566d-1152-2852-5a0a1a57368e"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("f911858b-88cd-7c16-f923-3c0a8c7b0260"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("fe0751d0-fdf4-6e60-5dda-b45f5e4967f9"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("ff95a46b-d616-d6a2-ddba-21506fc963f2"), new Guid("11111111-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("049b4483-ce6a-35c1-c766-2601f8552cef"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("1ae7500c-7a1c-58c5-1fcf-920141b79c57"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("1e71a3fa-0d38-b71a-18a6-0ca32ed8de0b"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("3682e3d3-4ff6-8c0d-72ab-ba20bc58e31d"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("455c5b15-5934-c2b1-d7c1-cd0daafbede1"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("53a9485b-0b9a-888c-8f80-73ec7b8d4c84"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("556ec1b1-7aeb-7091-51b2-3f00f194ae80"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("6f17ae43-ba84-9a2a-2c16-10223715ff7b"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("7347c061-93f9-2981-b53d-d999ce39d021"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("79e23999-58c4-8219-234b-a6a3a9172e92"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("8c0b81b0-84c5-b25b-8699-6ee51997f255"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("95185dfc-4aea-c4d8-66c5-77235e0c3075"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("b5dfd67b-408e-aa3b-0c71-d79be7fab61f"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("be48da10-f3dc-48e1-2112-a8eb0002f8ba"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("be94b080-b979-2550-66ac-8cb84826e55a"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("c8ace02b-0da5-71ee-e024-22cd3e485a9c"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("d125ccf7-474b-e193-0883-221b06dea83b"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("e5d436e7-73e7-09ca-d02b-fb337d790ad9"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("ed4f3f7f-eb00-1e61-003c-d0cb353bb39d"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("f38228da-566d-1152-2852-5a0a1a57368e"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("f911858b-88cd-7c16-f923-3c0a8c7b0260"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("fe0751d0-fdf4-6e60-5dda-b45f5e4967f9"), new Guid("11111111-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("3682e3d3-4ff6-8c0d-72ab-ba20bc58e31d"), new Guid("11111111-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("53a9485b-0b9a-888c-8f80-73ec7b8d4c84"), new Guid("11111111-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("556ec1b1-7aeb-7091-51b2-3f00f194ae80"), new Guid("11111111-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("79e23999-58c4-8219-234b-a6a3a9172e92"), new Guid("11111111-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("8c0b81b0-84c5-b25b-8699-6ee51997f255"), new Guid("11111111-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("b5dfd67b-408e-aa3b-0c71-d79be7fab61f"), new Guid("11111111-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("c8ace02b-0da5-71ee-e024-22cd3e485a9c"), new Guid("11111111-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("d125ccf7-474b-e193-0883-221b06dea83b"), new Guid("11111111-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("e5d436e7-73e7-09ca-d02b-fb337d790ad9"), new Guid("11111111-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("f38228da-566d-1152-2852-5a0a1a57368e"), new Guid("11111111-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("f911858b-88cd-7c16-f923-3c0a8c7b0260"), new Guid("11111111-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("3682e3d3-4ff6-8c0d-72ab-ba20bc58e31d"), new Guid("11111111-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("53a9485b-0b9a-888c-8f80-73ec7b8d4c84"), new Guid("11111111-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("556ec1b1-7aeb-7091-51b2-3f00f194ae80"), new Guid("11111111-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("79e23999-58c4-8219-234b-a6a3a9172e92"), new Guid("11111111-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("8c0b81b0-84c5-b25b-8699-6ee51997f255"), new Guid("11111111-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("f38228da-566d-1152-2852-5a0a1a57368e"), new Guid("11111111-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("f911858b-88cd-7c16-f923-3c0a8c7b0260"), new Guid("11111111-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("53a9485b-0b9a-888c-8f80-73ec7b8d4c84"), new Guid("11111111-0000-0000-0000-000000000005") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("556ec1b1-7aeb-7091-51b2-3f00f194ae80"), new Guid("11111111-0000-0000-0000-000000000005") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("8c0b81b0-84c5-b25b-8699-6ee51997f255"), new Guid("11111111-0000-0000-0000-000000000005") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("f38228da-566d-1152-2852-5a0a1a57368e"), new Guid("11111111-0000-0000-0000-000000000005") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("f911858b-88cd-7c16-f923-3c0a8c7b0260"), new Guid("11111111-0000-0000-0000-000000000005") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("049b4483-ce6a-35c1-c766-2601f8552cef"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("1ae7500c-7a1c-58c5-1fcf-920141b79c57"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("1e71a3fa-0d38-b71a-18a6-0ca32ed8de0b"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("3682e3d3-4ff6-8c0d-72ab-ba20bc58e31d"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("4091df6a-0088-e339-8960-a9d2a5910c99"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("455c5b15-5934-c2b1-d7c1-cd0daafbede1"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("53a9485b-0b9a-888c-8f80-73ec7b8d4c84"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("556ec1b1-7aeb-7091-51b2-3f00f194ae80"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("6f17ae43-ba84-9a2a-2c16-10223715ff7b"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("7347c061-93f9-2981-b53d-d999ce39d021"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("79e23999-58c4-8219-234b-a6a3a9172e92"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("8c0b81b0-84c5-b25b-8699-6ee51997f255"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("95185dfc-4aea-c4d8-66c5-77235e0c3075"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("b5dfd67b-408e-aa3b-0c71-d79be7fab61f"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("be48da10-f3dc-48e1-2112-a8eb0002f8ba"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("be94b080-b979-2550-66ac-8cb84826e55a"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("c8ace02b-0da5-71ee-e024-22cd3e485a9c"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("d125ccf7-474b-e193-0883-221b06dea83b"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("e5d436e7-73e7-09ca-d02b-fb337d790ad9"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("ed4f3f7f-eb00-1e61-003c-d0cb353bb39d"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("f38228da-566d-1152-2852-5a0a1a57368e"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("f911858b-88cd-7c16-f923-3c0a8c7b0260"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("fe0751d0-fdf4-6e60-5dda-b45f5e4967f9"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("ff95a46b-d616-d6a2-ddba-21506fc963f2"));
        }
    }
}
