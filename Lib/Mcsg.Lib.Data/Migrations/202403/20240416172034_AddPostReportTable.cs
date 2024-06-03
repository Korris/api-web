using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202403
{
    /// <inheritdoc />
    public partial class AddPostReportTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReasonType = table.Column<int>(type: "integer", nullable: false),
                    ReasonText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostReports_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostReports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostReports_PostId",
                table: "PostReports",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReports_UserId",
                table: "PostReports",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostReports");
        }
    }
}
