using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202310
{
    /// <inheritdoc />
    public partial class Add_SeedData_Email_Setting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsActive", "Key", "LastModifiedBy", "LastModifiedDate", "Value" },
                values: new object[] { new Guid("058ac52f-30ff-4708-a698-e10f612803da"), null, new DateTime(2023, 10, 17, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), true, "Email_Setting", null, new DateTime(2023, 10, 17, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), "{\"Host\":\"\",\"Port\":\"0\",\"Email\":\"\",\"Password\":\"Yq749dEmWRwavID/AW56pA==\",\"DisplayName\":\"\"}" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("058ac52f-30ff-4708-a698-e10f612803da"));
        }
    }
}
