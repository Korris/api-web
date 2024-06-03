using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202311
{
    /// <inheritdoc />
    public partial class UpdateNotificationStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "NotificationObjects");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Notifications");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "NotificationObjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
