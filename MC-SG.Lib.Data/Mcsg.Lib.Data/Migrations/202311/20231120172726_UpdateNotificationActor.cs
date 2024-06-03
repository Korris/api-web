using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202311
{
    /// <inheritdoc />
    public partial class UpdateNotificationActor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationObjects_NotificationActors_NotificationObjectId",
                table: "NotificationObjects");

            migrationBuilder.DropTable(
                name: "NotificationActors");

            migrationBuilder.DropIndex(
                name: "IX_NotificationObjects_NotificationObjectId",
                table: "NotificationObjects");

            migrationBuilder.DropColumn(
                name: "NotificationObjectId",
                table: "NotificationObjects");

            migrationBuilder.AddColumn<Guid>(
                name: "ActorId",
                table: "NotificationObjects",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActorId",
                table: "NotificationObjects");

            migrationBuilder.AddColumn<Guid>(
                name: "NotificationObjectId",
                table: "NotificationObjects",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NotificationActors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ActorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NotificationObjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationActors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationActors_Users_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationObjects_NotificationObjectId",
                table: "NotificationObjects",
                column: "NotificationObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationActors_ActorId",
                table: "NotificationActors",
                column: "ActorId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationObjects_NotificationActors_NotificationObjectId",
                table: "NotificationObjects",
                column: "NotificationObjectId",
                principalTable: "NotificationActors",
                principalColumn: "Id");
        }
    }
}
