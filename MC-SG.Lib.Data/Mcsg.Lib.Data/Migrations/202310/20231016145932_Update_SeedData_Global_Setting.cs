using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202310
{
    /// <inheritdoc />
    public partial class Update_SeedData_Global_Setting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("71fb8254-756b-4121-ac2f-01e87b25a673"),
                column: "Value",
                value: "{\"Favicon\":\"\",\"Title\":\"\",\"Desciption\":\"\"}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("71fb8254-756b-4121-ac2f-01e87b25a673"),
                column: "Value",
                value: "{\"Favicon\":\"\",\"Title\":\"\",\"Meta\":true,\"EmailSender\":\"\"}");
        }
    }
}
