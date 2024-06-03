using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mcsg.Lib.Data.Migrations._202311
{
    /// <inheritdoc />
    public partial class AddBackgroundMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BackgroundMedias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    Thumbnail = table.Column<string>(type: "text", nullable: true),
                    ArtistName = table.Column<string>(type: "text", nullable: true),
                    DurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundMedias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BackgroundMediaPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    BackgroundMediaId = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundMediaPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BackgroundMediaPosts_BackgroundMedias_BackgroundMediaId",
                        column: x => x.BackgroundMediaId,
                        principalTable: "BackgroundMedias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BackgroundMediaPosts_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BackgroundMedias",
                columns: new[] { "Id", "ArtistName", "CreatedBy", "CreatedDate", "DurationSeconds", "IsDelete", "LastModifiedBy", "LastModifiedDate", "Order", "Thumbnail", "Title", "Url" },
                values: new object[,]
                {
                    { new Guid("00c9697b-b475-421b-adb3-e736d22aea91"), "Game studio", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4957), 7, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4957), 7, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Ident", "fhRhp4yR4esQ24%2bXiThmBSkAHHW1nUzEdVvszykm9PYfOJ8daeY%2fHvicD%2bBbEPbS%2f5gcetEDGlVSGXZMobbnZQ%3d%3d" },
                    { new Guid("1e79a69f-3f4a-4f7e-9e57-ba3f0e154531"), "Audio Philetrax", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4963), 172, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4963), 10, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Dramatic Uplifting Cinematic Piano Trailer", "fhRhp4yR4esQ24%2bXiThmBXr4gGR3SKn%2fxMcnqOa4GEsiLwogqleCgjn41%2bqlxix%2fLP0nf38wcrAimWXOJ8DVPA%3d%3d" },
                    { new Guid("1ec4eef9-63a0-4035-8a16-e5292919007f"), "Fx", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4949), 25, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4949), 4, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Transition", "fhRhp4yR4esQ24%2bXiThmBTgrO9tGOX67wchuT4rghZrlGVHXqs6x%2bod1Y18Hie2BQHSXUYY%2f8banbA3TSv2xFA%3d%3d" },
                    { new Guid("3d16a45e-d73b-4526-a615-6aaeba4a087d"), "Music hunter", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4968), 924, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4968), 12, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBeLMpHV9AStRtNRLC1h6tf9J6QHdKXgWs4gfDXfoHcHcTnnZBIPf%2ftW74Eb48gAPLA%3d%3d" },
                    { new Guid("4c61b903-77aa-43e3-ab57-44d0556863ac"), "Isakukageyama", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4951), 30, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4951), 5, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Japanese Chillout", "fhRhp4yR4esQ24%2bXiThmBYn8pgA%2fXOWV0UX8B%2bX%2f1fFGxZvZ6XninqIg%2bUVnuUQ3mflbub%2bx7pitHJBozZl9xw%3d%3d" },
                    { new Guid("54a56568-4830-425c-b0ad-bb30dbb6c919"), "Bestandbless", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4975), 149, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4975), 15, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Coporate Motivation", "fhRhp4yR4esQ24%2bXiThmBY2SyUf0iMGN6P6juT0JJtYN7PfQC3WlABHpJfEszJigWiIqWyMxVomJje3l%2bZj05Q%3d%3d" },
                    { new Guid("80d8687a-7630-4f90-aa23-6f523075b9dd"), "Topflow", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4953), 120, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4953), 6, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Wedding Piano", "fhRhp4yR4esQ24%2bXiThmBT9GsBFxX2O7xsAKolnwlp2zohaeCE%2bRPRhYIvu7CGLvG0uaBVqYTD8p0hUhBl1t4Q%3d%3d" },
                    { new Guid("88c55861-cdf6-4f26-a95d-941e8e6001b0"), "Live Art", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4959), 242, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4959), 8, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivational corporate", "fhRhp4yR4esQ24%2bXiThmBergzRanh%2fVjrAgrDUq%2fG20PfNaBQ3HWJhLsUJF5ibvCLh0dz6SoGKAJTEbqxzKvTQ%3d%3d" },
                    { new Guid("ab9d4a3a-4bd3-4497-876e-accedf7a8021"), "Audio Chameleon", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4947), 175, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4947), 3, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Hopeful Piano", "fhRhp4yR4esQ24%2bXiThmBSZ4HmEt3fmIy%2f9dO4g7zm8LjJEYxuldCcOgtnRQPWhxxPbRDJ9FfEUqaJB87omgAw%3d%3d" },
                    { new Guid("b600e8f6-aaf6-4ab8-a33c-a14dc01098fd"), "Yetiproduction", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4970), 119, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4970), 13, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Inspiring Piano Motivation Cinematic Trailer", "fhRhp4yR4esQ24%2bXiThmBVLbsvkoYkePEeIbQni0GWm1Tss%2f0mDtIr%2bDj6P%2b0zgt51hRBOQeihzibJOyewO42A%3d%3d" },
                    { new Guid("c937563e-8933-4f4f-ba1c-a6a379a48c8a"), "Twinkle", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4942), 171, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4942), 1, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Relaxing Music", "fhRhp4yR4esQ24%2bXiThmBf4rRaxJPHn79%2beriEk3iHplANMkwzmK%2bmzCukOvj5YPt%2f3g7Kxdf7zCIRlw1ggAaQ%3d%3d" },
                    { new Guid("df30b294-1022-4501-a47b-cfa4102712db"), "Lexpremium", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4972), 144, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4972), 14, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Ambient Atmospheric Electronica", "fhRhp4yR4esQ24%2bXiThmBfNaG3vMj6TCthetecN24LkoHp4UibNof5rclnB7%2bF2O5VZDP2rfWMXhIzd8sS6OuA%3d%3d" },
                    { new Guid("e1d4b2a3-d22e-4784-a48d-e12df512ea70"), "Silverhoof", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4961), 470, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4961), 9, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBVABxTY%2bZOF3cgeCR1Dmq3AQTIYCDJr0Kq88Ehk%2bCgr4sbdqF1oiJ96J52IA8C7qeQ%3d%3d" },
                    { new Guid("e59b0ee3-c925-4343-ba5b-904ef1862e41"), "Upthemusic", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4966), 142, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4966), 11, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivation Uplifting", "fhRhp4yR4esQ24%2bXiThmBVLAI9PsjS2OtnyqQHYNwlOeUqyLDXiMnqcK2pXh6xBh9Vs1feAl0yp9hx0FEm2SSg%3d%3d" },
                    { new Guid("ee84f9aa-3780-40e2-ae28-7593e1b95b68"), "Luna", null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4938), 62, false, null, new DateTime(2023, 11, 21, 14, 19, 57, 990, DateTimeKind.Utc).AddTicks(4938), 0, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Tibet Zen Bell", "fhRhp4yR4esQ24%2bXiThmBe5WCn1AgyZ%2bDLXFiFkoAoVqDEXT6%2fBEepM0SvviUMhGevCG5haur%2b6%2bMf6Sv5s8ww%3d%3d" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundMediaPosts_BackgroundMediaId",
                table: "BackgroundMediaPosts",
                column: "BackgroundMediaId");

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundMediaPosts_PostId",
                table: "BackgroundMediaPosts",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackgroundMediaPosts");

            migrationBuilder.DropTable(
                name: "BackgroundMedias");
        }
    }
}
