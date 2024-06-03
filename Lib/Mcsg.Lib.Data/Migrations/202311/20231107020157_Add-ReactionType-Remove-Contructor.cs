using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202311
{
    /// <inheritdoc />
    public partial class AddReactionTypeRemoveContructor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedBy",
                table: "SystemSettings",
                type: "uuid using \"LastModifiedBy\"::uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "SystemSettings",
                type: "uuid using \"CreatedBy\"::uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "SystemSettings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("058ac52f-30ff-4708-a698-e10f612803da"),
                columns: new[] { "CreatedBy", "IsDelete", "LastModifiedBy" },
                values: new object[] { null, false, null });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("71fb8254-756b-4121-ac2f-01e87b25a673"),
                columns: new[] { "CreatedBy", "IsDelete", "LastModifiedBy" },
                values: new object[] { null, false, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "SystemSettings");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "SystemSettings",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "SystemSettings",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("058ac52f-30ff-4708-a698-e10f612803da"),
                columns: new[] { "CreatedBy", "LastModifiedBy" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("71fb8254-756b-4121-ac2f-01e87b25a673"),
                columns: new[] { "CreatedBy", "LastModifiedBy" },
                values: new object[] { null, null });
        }
    }
}
