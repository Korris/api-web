using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Wallet.Migrations._202312
{
    /// <inheritdoc />
    public partial class UpdateaddWalletSettingDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WalletSettingDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletSettingDetails", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "WalletSettingDetails",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "IsDelete", "ModifiedBy", "ModifiedDate", "Name", "Type", "Value" },
                values: new object[] { new Guid("5f65fed1-bddc-4c7c-ba57-cb33a54542c8"), null, new DateTime(2023, 12, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), "With draw notify list email", false, null, new DateTime(2023, 12, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), "WithDrawNotify", 0, "dev@angelpj.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletSettingDetails");
        }
    }
}
