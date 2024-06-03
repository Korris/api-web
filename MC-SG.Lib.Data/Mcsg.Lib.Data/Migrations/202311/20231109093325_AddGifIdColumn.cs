using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202311
{
    /// <inheritdoc />
    public partial class AddGifIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GifId",
                table: "SubPostComments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GifId",
                table: "PostComments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GifId",
                table: "SubPostComments");

            migrationBuilder.DropColumn(
                name: "GifId",
                table: "PostComments");
        }
    }
}
