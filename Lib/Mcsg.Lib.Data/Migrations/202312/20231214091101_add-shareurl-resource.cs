using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202312
{
    /// <inheritdoc />
    public partial class addshareurlresource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShareUrl",
                table: "Resources",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("0ca3fad1-9ffc-463f-a272-12857fb45449"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("18328fa2-1405-465f-bd10-fd10a9597d72"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("37292db9-1c27-436d-9dfe-3d4128cc9cbd"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("492796c5-9415-4ddc-9ff4-0957158a4f69"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("4bee64d9-19b0-4c78-98d5-ff282091a448"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("56abcbd1-4bf7-4ed7-bcb7-e761e01ae31b"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("5dcd89de-3377-428b-9f33-aa2b0b054c77"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("698c1058-bd20-404c-bc4e-e67450f5ce85"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("716f89ca-25ba-4bd8-8757-a80f56969ba9"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("8fa99fda-1cce-4e61-95fa-2b820f3e6ab8"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("a52233ca-512c-4d3a-85f1-e087a2234ed7"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("b0bc57d8-a3ab-4b7a-82a5-43d8e563a8e2"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("c1c4a924-c325-419d-815f-db9b0ba728b1"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("f854249b-1b0a-4a30-b255-d62d315da257"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6510) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShareUrl",
                table: "Resources");

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("0ca3fad1-9ffc-463f-a272-12857fb45449"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6515), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6515) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("18328fa2-1405-465f-bd10-fd10a9597d72"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6519), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6519) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("37292db9-1c27-436d-9dfe-3d4128cc9cbd"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6522), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6522) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("492796c5-9415-4ddc-9ff4-0957158a4f69"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6528), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6528) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("4bee64d9-19b0-4c78-98d5-ff282091a448"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6531), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6531) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("56abcbd1-4bf7-4ed7-bcb7-e761e01ae31b"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6534), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6534) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("5dcd89de-3377-428b-9f33-aa2b0b054c77"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6537), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6537) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("698c1058-bd20-404c-bc4e-e67450f5ce85"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6539), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6539) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("716f89ca-25ba-4bd8-8757-a80f56969ba9"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6542), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6542) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("8fa99fda-1cce-4e61-95fa-2b820f3e6ab8"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6548), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6548) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("a52233ca-512c-4d3a-85f1-e087a2234ed7"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6552), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6552) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("b0bc57d8-a3ab-4b7a-82a5-43d8e563a8e2"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6557), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6557) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("c1c4a924-c325-419d-815f-db9b0ba728b1"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6560), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6560) });

            migrationBuilder.UpdateData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("f854249b-1b0a-4a30-b255-d62d315da257"),
                columns: new[] { "CreatedDate", "LastModifiedDate" },
                values: new object[] { new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6563), new DateTime(2023, 12, 13, 8, 18, 34, 709, DateTimeKind.Utc).AddTicks(6563) });
        }
    }
}
