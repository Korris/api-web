using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202401
{
    /// <inheritdoc />
    public partial class AddExternalCodeToSubPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalCode",
                table: "SubPosts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExternalResource",
                table: "SubPosts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalCode",
                table: "SubPosts");

            migrationBuilder.DropColumn(
                name: "ExternalResource",
                table: "SubPosts");
        }
    }
}
