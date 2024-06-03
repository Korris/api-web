using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202310
{
    /// <inheritdoc />
    public partial class Update_SeedData_Default_Password_For_Email_Setting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("058ac52f-30ff-4708-a698-e10f612803da"),
                column: "Value",
                value: "{\"Host\":\"\",\"Port\":0,\"Email\":\"\",\"Password\":\"IBZfY0Bry0MIPxfL1\\u002Biolw==\",\"DisplayName\":\"\"}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("058ac52f-30ff-4708-a698-e10f612803da"),
                column: "Value",
                value: "{\"Host\":\"\",\"Port\":\"0\",\"Email\":\"\",\"Password\":\"Yq749dEmWRwavID/AW56pA==\",\"DisplayName\":\"\"}");
        }
    }
}
