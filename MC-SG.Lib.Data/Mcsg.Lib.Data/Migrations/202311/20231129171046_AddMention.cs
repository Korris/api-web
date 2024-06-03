using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mcsg.Lib.Data.Migrations._202311
{
    /// <inheritdoc />
    public partial class AddMention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("0ae1b13f-ef3b-4d0b-ade5-27db688d623d"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("12e4ab10-cbe6-4eb7-8daf-3f74650c23fa"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("16ab9864-ad1e-4d3e-acad-05261a97830b"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("26351c57-1896-4e37-b5e8-876575dd4203"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("32fbb6a0-6a23-44bb-ba8a-16734d6f5c3f"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("433a03f3-ca14-4eec-b928-0b13b866bf8d"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("6813ec0d-09f9-49fb-bda4-409921df7eb5"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("8c7bfaa1-7236-420f-bb6e-c407482dfbc3"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("97470004-9aee-4fdd-8642-2ce582850259"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("e156738a-e43f-4d13-9240-1efc4693fbca"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("e61a530d-a505-4287-8dff-45a7d2964e14"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("e6a7d995-893b-4dcb-b9ef-775178d7dad9"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("eb8feb18-f093-4b36-b682-33b73bae9854"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("ecd50935-20c2-47cc-b6a8-d7fae482a7b0"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("f3548131-f69d-4111-9f13-6c703ae27991"));

            migrationBuilder.CreateTable(
                name: "Mentions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationType = table.Column<int>(type: "integer", nullable: false),
                    UserMentionedId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mentions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mentions_Users_UserMentionedId",
                        column: x => x.UserMentionedId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BackgroundMedias",
                columns: new[] { "Id", "ArtistName", "CreatedBy", "CreatedDate", "DurationSeconds", "IsDelete", "LastModifiedBy", "LastModifiedDate", "Order", "Thumbnail", "Title", "Url" },
                values: new object[,]
                {
                    { new Guid("026a0080-4de8-40d2-93f1-873a1912e6ee"), "Audio Chameleon", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5386), 175, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5386), 3, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Hopeful Piano", "fhRhp4yR4esQ24%2bXiThmBSZ4HmEt3fmIy%2f9dO4g7zm8LjJEYxuldCcOgtnRQPWhxxPbRDJ9FfEUqaJB87omgAw%3d%3d" },
                    { new Guid("230e6b9d-765d-4eea-a331-3f7e2edb3927"), "Isakukageyama", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5391), 30, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5391), 5, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Japanese Chillout", "fhRhp4yR4esQ24%2bXiThmBYn8pgA%2fXOWV0UX8B%2bX%2f1fFGxZvZ6XninqIg%2bUVnuUQ3mflbub%2bx7pitHJBozZl9xw%3d%3d" },
                    { new Guid("34662348-cf76-4b7c-9ea6-747fc55f07f6"), "Lexpremium", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5422), 144, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5422), 14, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Ambient Atmospheric Electronica", "fhRhp4yR4esQ24%2bXiThmBfNaG3vMj6TCthetecN24LkoHp4UibNof5rclnB7%2bF2O5VZDP2rfWMXhIzd8sS6OuA%3d%3d" },
                    { new Guid("49970cbe-5644-4a15-b57a-5f65da2e4f64"), "Audio Philetrax", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5408), 172, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5408), 10, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Dramatic Uplifting Cinematic Piano Trailer", "fhRhp4yR4esQ24%2bXiThmBXr4gGR3SKn%2fxMcnqOa4GEsiLwogqleCgjn41%2bqlxix%2fLP0nf38wcrAimWXOJ8DVPA%3d%3d" },
                    { new Guid("560ab2b4-5f68-4c9a-9d8b-bdc2469e1829"), "Game studio", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5400), 7, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5400), 7, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Ident", "fhRhp4yR4esQ24%2bXiThmBSkAHHW1nUzEdVvszykm9PYfOJ8daeY%2fHvicD%2bBbEPbS%2f5gcetEDGlVSGXZMobbnZQ%3d%3d" },
                    { new Guid("5f95ffe7-71e4-4c89-9c5a-25fcdb555c0c"), "Twinkle", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5377), 171, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5377), 1, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Relaxing Music", "fhRhp4yR4esQ24%2bXiThmBf4rRaxJPHn79%2beriEk3iHplANMkwzmK%2bmzCukOvj5YPt%2f3g7Kxdf7zCIRlw1ggAaQ%3d%3d" },
                    { new Guid("641744f9-ef8d-4f24-8072-7fcbcd22d594"), "Upthemusic", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5413), 142, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5413), 11, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivation Uplifting", "fhRhp4yR4esQ24%2bXiThmBVLAI9PsjS2OtnyqQHYNwlOeUqyLDXiMnqcK2pXh6xBh9Vs1feAl0yp9hx0FEm2SSg%3d%3d" },
                    { new Guid("7f32a80e-e473-4446-a928-d8a13fdc6bb4"), "Fx", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5389), 25, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5389), 4, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Transition", "fhRhp4yR4esQ24%2bXiThmBTgrO9tGOX67wchuT4rghZrlGVHXqs6x%2bod1Y18Hie2BQHSXUYY%2f8banbA3TSv2xFA%3d%3d" },
                    { new Guid("8a2f4457-31a8-4bd1-b089-6424916ecfdc"), "Yetiproduction", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5419), 119, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5419), 13, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Inspiring Piano Motivation Cinematic Trailer", "fhRhp4yR4esQ24%2bXiThmBVLbsvkoYkePEeIbQni0GWm1Tss%2f0mDtIr%2bDj6P%2b0zgt51hRBOQeihzibJOyewO42A%3d%3d" },
                    { new Guid("93668a57-4ed0-4043-a9cc-02039571a8da"), "Luna", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5371), 62, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5371), 0, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Tibet Zen Bell", "fhRhp4yR4esQ24%2bXiThmBe5WCn1AgyZ%2bDLXFiFkoAoVqDEXT6%2fBEepM0SvviUMhGevCG5haur%2b6%2bMf6Sv5s8ww%3d%3d" },
                    { new Guid("9eca3180-bea8-4fd6-bcef-6f1383b5d15a"), "Music hunter", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5416), 924, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5416), 12, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBeLMpHV9AStRtNRLC1h6tf9J6QHdKXgWs4gfDXfoHcHcTnnZBIPf%2ftW74Eb48gAPLA%3d%3d" },
                    { new Guid("b562f597-32bf-46e2-8193-84f56480cb6c"), "Silverhoof", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5406), 470, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5406), 9, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBVABxTY%2bZOF3cgeCR1Dmq3AQTIYCDJr0Kq88Ehk%2bCgr4sbdqF1oiJ96J52IA8C7qeQ%3d%3d" },
                    { new Guid("d9740c4f-3c28-42a8-9d12-913a05cf66e7"), "Topflow", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5395), 120, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5395), 6, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Wedding Piano", "fhRhp4yR4esQ24%2bXiThmBT9GsBFxX2O7xsAKolnwlp2zohaeCE%2bRPRhYIvu7CGLvG0uaBVqYTD8p0hUhBl1t4Q%3d%3d" },
                    { new Guid("dce625bf-a7e1-4630-ac32-07bb877cdda5"), "Live Art", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5403), 242, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5403), 8, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivational corporate", "fhRhp4yR4esQ24%2bXiThmBergzRanh%2fVjrAgrDUq%2fG20PfNaBQ3HWJhLsUJF5ibvCLh0dz6SoGKAJTEbqxzKvTQ%3d%3d" },
                    { new Guid("ddf6e105-d468-4c5c-bd52-4602cd98d000"), "Bestandbless", null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5427), 149, false, null, new DateTime(2023, 11, 29, 17, 10, 46, 292, DateTimeKind.Utc).AddTicks(5427), 15, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Coporate Motivation", "fhRhp4yR4esQ24%2bXiThmBY2SyUf0iMGN6P6juT0JJtYN7PfQC3WlABHpJfEszJigWiIqWyMxVomJje3l%2bZj05Q%3d%3d" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mentions_UserMentionedId",
                table: "Mentions",
                column: "UserMentionedId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mentions");

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("026a0080-4de8-40d2-93f1-873a1912e6ee"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("230e6b9d-765d-4eea-a331-3f7e2edb3927"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("34662348-cf76-4b7c-9ea6-747fc55f07f6"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("49970cbe-5644-4a15-b57a-5f65da2e4f64"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("560ab2b4-5f68-4c9a-9d8b-bdc2469e1829"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("5f95ffe7-71e4-4c89-9c5a-25fcdb555c0c"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("641744f9-ef8d-4f24-8072-7fcbcd22d594"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("7f32a80e-e473-4446-a928-d8a13fdc6bb4"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("8a2f4457-31a8-4bd1-b089-6424916ecfdc"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("93668a57-4ed0-4043-a9cc-02039571a8da"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("9eca3180-bea8-4fd6-bcef-6f1383b5d15a"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("b562f597-32bf-46e2-8193-84f56480cb6c"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("d9740c4f-3c28-42a8-9d12-913a05cf66e7"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("dce625bf-a7e1-4630-ac32-07bb877cdda5"));

            migrationBuilder.DeleteData(
                table: "BackgroundMedias",
                keyColumn: "Id",
                keyValue: new Guid("ddf6e105-d468-4c5c-bd52-4602cd98d000"));

            migrationBuilder.InsertData(
                table: "BackgroundMedias",
                columns: new[] { "Id", "ArtistName", "CreatedBy", "CreatedDate", "DurationSeconds", "IsDelete", "LastModifiedBy", "LastModifiedDate", "Order", "Thumbnail", "Title", "Url" },
                values: new object[,]
                {
                    { new Guid("0ae1b13f-ef3b-4d0b-ade5-27db688d623d"), "Luna", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6392), 62, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6392), 0, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Tibet Zen Bell", "fhRhp4yR4esQ24%2bXiThmBe5WCn1AgyZ%2bDLXFiFkoAoVqDEXT6%2fBEepM0SvviUMhGevCG5haur%2b6%2bMf6Sv5s8ww%3d%3d" },
                    { new Guid("12e4ab10-cbe6-4eb7-8daf-3f74650c23fa"), "Twinkle", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6396), 171, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6396), 1, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Relaxing Music", "fhRhp4yR4esQ24%2bXiThmBf4rRaxJPHn79%2beriEk3iHplANMkwzmK%2bmzCukOvj5YPt%2f3g7Kxdf7zCIRlw1ggAaQ%3d%3d" },
                    { new Guid("16ab9864-ad1e-4d3e-acad-05261a97830b"), "Music hunter", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6422), 924, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6422), 12, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBeLMpHV9AStRtNRLC1h6tf9J6QHdKXgWs4gfDXfoHcHcTnnZBIPf%2ftW74Eb48gAPLA%3d%3d" },
                    { new Guid("26351c57-1896-4e37-b5e8-876575dd4203"), "Audio Philetrax", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6417), 172, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6417), 10, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Dramatic Uplifting Cinematic Piano Trailer", "fhRhp4yR4esQ24%2bXiThmBXr4gGR3SKn%2fxMcnqOa4GEsiLwogqleCgjn41%2bqlxix%2fLP0nf38wcrAimWXOJ8DVPA%3d%3d" },
                    { new Guid("32fbb6a0-6a23-44bb-ba8a-16734d6f5c3f"), "Audio Chameleon", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6399), 175, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6399), 3, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Hopeful Piano", "fhRhp4yR4esQ24%2bXiThmBSZ4HmEt3fmIy%2f9dO4g7zm8LjJEYxuldCcOgtnRQPWhxxPbRDJ9FfEUqaJB87omgAw%3d%3d" },
                    { new Guid("433a03f3-ca14-4eec-b928-0b13b866bf8d"), "Game studio", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6410), 7, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6410), 7, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Ident", "fhRhp4yR4esQ24%2bXiThmBSkAHHW1nUzEdVvszykm9PYfOJ8daeY%2fHvicD%2bBbEPbS%2f5gcetEDGlVSGXZMobbnZQ%3d%3d" },
                    { new Guid("6813ec0d-09f9-49fb-bda4-409921df7eb5"), "Live Art", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6413), 242, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6413), 8, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivational corporate", "fhRhp4yR4esQ24%2bXiThmBergzRanh%2fVjrAgrDUq%2fG20PfNaBQ3HWJhLsUJF5ibvCLh0dz6SoGKAJTEbqxzKvTQ%3d%3d" },
                    { new Guid("8c7bfaa1-7236-420f-bb6e-c407482dfbc3"), "Silverhoof", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6415), 470, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6415), 9, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen", "fhRhp4yR4esQ24%2bXiThmBVABxTY%2bZOF3cgeCR1Dmq3AQTIYCDJr0Kq88Ehk%2bCgr4sbdqF1oiJ96J52IA8C7qeQ%3d%3d" },
                    { new Guid("97470004-9aee-4fdd-8642-2ce582850259"), "Isakukageyama", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6407), 30, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6407), 5, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Japanese Chillout", "fhRhp4yR4esQ24%2bXiThmBYn8pgA%2fXOWV0UX8B%2bX%2f1fFGxZvZ6XninqIg%2bUVnuUQ3mflbub%2bx7pitHJBozZl9xw%3d%3d" },
                    { new Guid("e156738a-e43f-4d13-9240-1efc4693fbca"), "Fx", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6404), 25, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6404), 4, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Zen Transition", "fhRhp4yR4esQ24%2bXiThmBTgrO9tGOX67wchuT4rghZrlGVHXqs6x%2bod1Y18Hie2BQHSXUYY%2f8banbA3TSv2xFA%3d%3d" },
                    { new Guid("e61a530d-a505-4287-8dff-45a7d2964e14"), "Upthemusic", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6419), 142, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6419), 11, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Motivation Uplifting", "fhRhp4yR4esQ24%2bXiThmBVLAI9PsjS2OtnyqQHYNwlOeUqyLDXiMnqcK2pXh6xBh9Vs1feAl0yp9hx0FEm2SSg%3d%3d" },
                    { new Guid("e6a7d995-893b-4dcb-b9ef-775178d7dad9"), "Bestandbless", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6430), 149, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6430), 15, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Coporate Motivation", "fhRhp4yR4esQ24%2bXiThmBY2SyUf0iMGN6P6juT0JJtYN7PfQC3WlABHpJfEszJigWiIqWyMxVomJje3l%2bZj05Q%3d%3d" },
                    { new Guid("eb8feb18-f093-4b36-b682-33b73bae9854"), "Topflow", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6408), 120, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6408), 6, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Emotional Inspiring Wedding Piano", "fhRhp4yR4esQ24%2bXiThmBT9GsBFxX2O7xsAKolnwlp2zohaeCE%2bRPRhYIvu7CGLvG0uaBVqYTD8p0hUhBl1t4Q%3d%3d" },
                    { new Guid("ecd50935-20c2-47cc-b6a8-d7fae482a7b0"), "Lexpremium", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6428), 144, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6428), 14, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Ambient Atmospheric Electronica", "fhRhp4yR4esQ24%2bXiThmBfNaG3vMj6TCthetecN24LkoHp4UibNof5rclnB7%2bF2O5VZDP2rfWMXhIzd8sS6OuA%3d%3d" },
                    { new Guid("f3548131-f69d-4111-9f13-6c703ae27991"), "Yetiproduction", null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6426), 119, false, null, new DateTime(2023, 11, 27, 3, 40, 12, 30, DateTimeKind.Utc).AddTicks(6426), 13, "fhRhp4yR4esQ24%2bXiThmBYQ6qfOOxWiqnGI7zQH29kO7yLbt1HzPsnLuWIasdLfBd5LMxgo4dSjFhCavUOSXnA%3d%3d", "Inspiring Piano Motivation Cinematic Trailer", "fhRhp4yR4esQ24%2bXiThmBVLbsvkoYkePEeIbQni0GWm1Tss%2f0mDtIr%2bDj6P%2b0zgt51hRBOQeihzibJOyewO42A%3d%3d" }
                });
        }
    }
}
