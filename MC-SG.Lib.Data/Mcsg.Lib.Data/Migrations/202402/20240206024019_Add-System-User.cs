using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202402
{
    /// <inheritdoc />
    public partial class AddSystemUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ActivedDate", "Avatar", "ConcurrencyStamp", "CoverPhoto", "CreatedBy", "CreatedDate", "DateOfBirth", "Email", "EmailConfirmed", "FirstName", "Gender", "IsDelete", "LastLoginDate", "LastModifiedBy", "LastModifiedDate", "LastName", "Location", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PremiumDate", "ProfileId", "ProfileName", "ReferralCode", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "Status", "StatusReason", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("00000000-0000-0000-0000-000000000001"), 0, null, null, "a625d884-4387-48cf-b584-3a9b1b228832", null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, "system@angelpj.com", false, null, null, false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), null, null, false, null, "SYSTEM@ANGELPJ.COM", "system@ANGELPJ.COM", "AQAAAAIAAYagAAAAEKr7Nj0sDqfYailaVLg2J+hlUf2FE+Y87N4gqQEuVuoXgW2t8Xf+FErCIkoVEKszQg==", null, false, null, null, null, null, null, null, "2c3a9e40-a16f-4b86-9e26-67a00fc939db", 1, null, false, "system@angelpj.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));
        }
    }
}
