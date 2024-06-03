using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mcsg.Lib.Data.Migrations._202310
{
    /// <inheritdoc />
    public partial class Update_SeedData_User_and_setting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2c71c3f0-1b62-4c88-b3ad-726c0a1d8018"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("a7be97c1-e049-4d42-b68b-4145e73dcaad"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("16caa60f-1c2b-435d-806a-875a7e54e3ab"), new Guid("24d0f964-933a-417f-9468-d8d81ffbbdcd") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("16caa60f-1c2b-435d-806a-875a7e54e3ab"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("24d0f964-933a-417f-9468-d8d81ffbbdcd"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("53a787ef-f614-4425-95ed-c1905a917b85"), null, "Mcsg.User", "MCSG.USER" },
                    { new Guid("ae2fac1e-dbbd-4ed9-b4c9-160375bf28d5"), null, "Mcsg.SysAdmin", "MCSG.SYSADMIN" },
                    { new Guid("b0632b0e-8ebd-4303-8ec6-e100ba4204e4"), null, "Mcsg.Admin", "MCSG.ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsActive", "Key", "LastModifiedBy", "LastModifiedDate", "Value" },
                values: new object[] { new Guid("71fb8254-756b-4121-ac2f-01e87b25a673"), null, new DateTime(2023, 10, 11, 7, 1, 14, 456, DateTimeKind.Utc).AddTicks(9535), true, "Global_Setting", null, new DateTime(2023, 10, 11, 7, 1, 14, 456, DateTimeKind.Utc).AddTicks(9509), "{\"Favicon\":\"\",\"Title\":\"\",\"Meta\":true,\"EmailSender\":\"\"}" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedBy", "CreatedDate", "DateOfBirth", "Email", "EmailConfirmed", "FirstName", "Gender", "IsDelete", "LastModifiedBy", "LastModifiedDate", "LastName", "LockoutEnabled", "LockoutEnd", "Nickname", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ReferralCode", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "Status", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb"), 0, null, "e26fb45d-8c83-412d-a9bb-528c97dd545e", null, new DateTime(2023, 10, 11, 7, 1, 14, 365, DateTimeKind.Utc).AddTicks(2433), null, null, false, null, null, false, null, new DateTime(2023, 10, 11, 7, 1, 14, 365, DateTimeKind.Utc).AddTicks(2433), null, false, null, null, null, "admin", "AQAAAAIAAYagAAAAEI52yKp7h/tMI5WVy9fvQDVCzolejSvN0k1YOnIT4iiqWALllSTrBuHhDj+cQSo+sg==", null, false, null, null, null, null, 1, false, "admin" },
                    { new Guid("ff5727ac-4b12-4e03-9e02-25bfb9086cbf"), 0, null, "b562756a-c8ec-45b9-a4e1-0a2845f30309", null, new DateTime(2023, 10, 11, 7, 1, 14, 412, DateTimeKind.Utc).AddTicks(652), null, null, false, null, null, false, null, new DateTime(2023, 10, 11, 7, 1, 14, 412, DateTimeKind.Utc).AddTicks(652), null, false, null, null, null, "sysadmin", "AQAAAAIAAYagAAAAEGX83QlTjUo/vsmTqdnHGVgKReuJMJL8/ufkoLgI+GOq1a5345r0EJaLVAbHH4BgKQ==", null, false, null, null, null, null, 1, false, "sysadmin" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("b0632b0e-8ebd-4303-8ec6-e100ba4204e4"), new Guid("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb") },
                    { new Guid("ae2fac1e-dbbd-4ed9-b4c9-160375bf28d5"), new Guid("ff5727ac-4b12-4e03-9e02-25bfb9086cbf") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("53a787ef-f614-4425-95ed-c1905a917b85"));

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("71fb8254-756b-4121-ac2f-01e87b25a673"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("b0632b0e-8ebd-4303-8ec6-e100ba4204e4"), new Guid("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("ae2fac1e-dbbd-4ed9-b4c9-160375bf28d5"), new Guid("ff5727ac-4b12-4e03-9e02-25bfb9086cbf") });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ae2fac1e-dbbd-4ed9-b4c9-160375bf28d5"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("b0632b0e-8ebd-4303-8ec6-e100ba4204e4"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff5727ac-4b12-4e03-9e02-25bfb9086cbf"));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("16caa60f-1c2b-435d-806a-875a7e54e3ab"), null, "Mcsg.SysAdmin", "MCSG.SYSADMIN" },
                    { new Guid("2c71c3f0-1b62-4c88-b3ad-726c0a1d8018"), null, "Mcsg.Admin", "MCSG.ADMIN" },
                    { new Guid("a7be97c1-e049-4d42-b68b-4145e73dcaad"), null, "Mcsg.User", "MCSG.USER" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "Avatar", "ConcurrencyStamp", "CreatedBy", "CreatedDate", "DateOfBirth", "Email", "EmailConfirmed", "FirstName", "Gender", "IsDelete", "LastModifiedBy", "LastModifiedDate", "LastName", "LockoutEnabled", "LockoutEnd", "Nickname", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ReferralCode", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "Status", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("24d0f964-933a-417f-9468-d8d81ffbbdcd"), 0, null, "20f62bab-7ccf-4ee1-af5f-cd05fc5c9b10", null, new DateTime(2023, 10, 9, 15, 51, 50, 294, DateTimeKind.Utc).AddTicks(124), null, null, false, null, null, false, null, new DateTime(2023, 10, 9, 15, 51, 50, 294, DateTimeKind.Utc).AddTicks(124), null, false, null, null, null, "admin", "AQAAAAIAAYagAAAAEMK1caCg2VlaUSpfd7EGnn7uoxCrCNK0VPxdyE2CwgZafZhs0RcHnrTHkSQOXns9dg==", null, false, null, null, null, null, 1, false, "admin" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("16caa60f-1c2b-435d-806a-875a7e54e3ab"), new Guid("24d0f964-933a-417f-9468-d8d81ffbbdcd") });
        }
    }
}
