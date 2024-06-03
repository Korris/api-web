using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202310
{
    /// <inheritdoc />
    public partial class fix_hard_auto_value : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("71fb8254-756b-4121-ac2f-01e87b25a673"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb"),
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "LastModifiedDate", "PasswordHash" },
                values: new object[] { "a625d884-4387-48cf-b584-3a9b1b228832", new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), "AQAAAAIAAYagAAAAEIvW0O+erQdOT5gTf8NMInqD6siJ/HJy+3cYsZ0V6NmQFcm4HWvysYxajMisoF6M5A==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff5727ac-4b12-4e03-9e02-25bfb9086cbf"),
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "LastModifiedDate", "PasswordHash" },
                values: new object[] { "a625d884-4387-48cf-b584-3a9b1b228832", new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), "AQAAAAIAAYagAAAAEKr7Nj0sDqfYailaVLg2J+hlUf2FE+Y87N4gqQEuVuoXgW2t8Xf+FErCIkoVEKszQg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("71fb8254-756b-4121-ac2f-01e87b25a673"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 10, 11, 7, 1, 14, 456, DateTimeKind.Utc).AddTicks(9535), new DateTime(2023, 10, 11, 7, 1, 14, 456, DateTimeKind.Utc).AddTicks(9509) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb"),
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "LastModifiedDate", "PasswordHash" },
                values: new object[] { "e26fb45d-8c83-412d-a9bb-528c97dd545e", new DateTime(2023, 10, 11, 7, 1, 14, 365, DateTimeKind.Utc).AddTicks(2433), new DateTime(2023, 10, 11, 7, 1, 14, 365, DateTimeKind.Utc).AddTicks(2433), "AQAAAAIAAYagAAAAEI52yKp7h/tMI5WVy9fvQDVCzolejSvN0k1YOnIT4iiqWALllSTrBuHhDj+cQSo+sg==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff5727ac-4b12-4e03-9e02-25bfb9086cbf"),
                columns: new[] { "ConcurrencyStamp", "CreatedDate", "LastModifiedDate", "PasswordHash" },
                values: new object[] { "b562756a-c8ec-45b9-a4e1-0a2845f30309", new DateTime(2023, 10, 11, 7, 1, 14, 412, DateTimeKind.Utc).AddTicks(652), new DateTime(2023, 10, 11, 7, 1, 14, 412, DateTimeKind.Utc).AddTicks(652), "AQAAAAIAAYagAAAAEGX83QlTjUo/vsmTqdnHGVgKReuJMJL8/ufkoLgI+GOq1a5345r0EJaLVAbHH4BgKQ==" });
        }
    }
}
